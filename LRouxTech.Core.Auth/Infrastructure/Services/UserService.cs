using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Core.ViewModels.User;
using LRouxTech.Core.Auth.Core.ViewModels.User.Reponse;
using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.Auth.Infrastructure.Errors;
using LRouxTech.Core.Auth.Infrastructure.Helper;
using LRouxTech.Core.Auth.Infrastructure.Templates;
using LRouxTech.Core.Auth.Infrastructure.Validator;
using LRouxTech.Core.Mail;
using LRouxTech.Core.ValidationResult;
using Microsoft.EntityFrameworkCore;

namespace LRouxTech.Core.Auth.Infrastructure.Services;

public class UserService(UserContext userContext, ITokenService tokenService, IUserValidator userValidator, IEmailService emailService)
    : IUserService
{
    public async Task<Result<SignedInUserResponse>> Login(UserLoginRequest request)
    {
        var user = await userContext.Users
            .Include(x => x.UserTokens.Where(x => !x.Expired && x.ExpiresOn > DateTime.Now))
            .Include(x => x.UserPermissions).Include(x => x.UserRoles)
            .FirstOrDefaultAsync(x => x.UserName == request.UserName);
        if (user == null)
        {
            return UserErrors.UserNotFound;
        }

        if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return UserErrors.InvalidPassword;
        }

        UserToken userToken;
        if (user.UserTokens.ToList() is null or [])
        {
            var tokenResult = await tokenService.GenerateToken(user.Id);
            if (tokenResult.IsFailure)
            {
                return tokenResult.Error;
            }

            userToken = tokenResult.Value;
        }
        else
        {
            userToken = user.UserTokens.First();
        }

        return new SignedInUserResponse(user.Id, user.UserName, user.Email, userToken.TokenValue, userToken.ExpiresOn,
            user.UserRoles.Select(x => x.RoleId).ToList(), user.UserPermissions.Select(x => x.PermissionId).ToList());
    }

    public async Task<Result<bool>> Logout(UserLogoutRequest request)
    {
        var user = await userContext.Users
            .Include(x => x.UserTokens.Where(x => !x.Expired && x.ExpiresOn > DateTime.Now))
            .FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (user == null)
        {
            return UserErrors.UserNotFound;
        }

        foreach (var token in user.UserTokens)
        {
            token.Expired = true;
            userContext.UserTokens.Update(token);
        }

        await userContext.SaveChangesAsync();
        return true;
    }

    public async Task<Result<User>> Create(CreateUserRequest request)
    {
        var validationResult = userValidator.ValidateUserCreation(request);
        if (validationResult.IsFailure)
        {
            return validationResult.Error;
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            UserName = request.Username,
            UserRoles = request.RoleIds.Select(x => new UserRole { RoleId = x }).ToList(),
            UserPermissions = request.PermissionIds.Select(x => new UserPermission { PermissionId = x }).ToList(),
        };
        userContext.Users.Add(user);
        await userContext.SaveChangesAsync();
        
                
        string htmlTemplate = LoadTemplate.LoadEmbeddedTemplate("WelcomeMail.html");

        string localizedBody = htmlTemplate
            .Replace("{ResetLink}", "https://yourdomain.co.za/auth/reset-password?token=xyz123");

        await emailService.SendEmailAsync(user.Email, "Reset Your Password", localizedBody);

        
        return user;
    }

    public async Task<Result<User>> Update(UpdateUserRequest request)
    {
        var validationResult = userValidator.ValidateUserUpdate(request);
        if (validationResult.IsFailure)
        {
            return validationResult.Error;
        }

        var user = await userContext.Users.Include(x => x.UserPermissions).Include(x => x.UserRoles)
            .FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (user == null)
        {
            return UserErrors.UserNotFound;
        }

        user.Name = request.Name;
        user.Email = request.Email;
        user.UserName = request.Username;
        var rolesToRemove = user.UserRoles.Where(ur => !request.RoleIds.Contains(ur.RoleId)).ToList();
        foreach (var role in rolesToRemove)
        {
            user.UserRoles.Remove(role);
        }

        var existingRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToHashSet();
        var rolesToAdd = request.RoleIds.Where(id => !existingRoleIds.Contains(id))
            .Select(id => new UserRole { UserId = user.Id, RoleId = id });
        foreach (var newRole in rolesToAdd)
        {
            user.UserRoles.Add(newRole);
        }

        var permissionsToRemove = user.UserPermissions
            .Where(up => !request.PermissionIds.Contains(up.PermissionId)).ToList();
        foreach (var permission in permissionsToRemove)
        {
            user.UserPermissions.Remove(permission);
        }

        var existingPermissionIds = user.UserPermissions.Select(up => up.PermissionId).ToHashSet();
        var permissionsToAdd = request.PermissionIds.Where(id => !existingPermissionIds.Contains(id))
            .Select(id => new UserPermission { UserId = user.Id, PermissionId = id });
        foreach (var newPermission in permissionsToAdd)
        {
            user.UserPermissions.Add(newPermission);
        }

        userContext.Users.Update(user);
        await userContext.SaveChangesAsync();
        return user;
    }

    public async Task<Result<SignedInUserResponse>> Authenticate(AuthenticateUserRequest request)
    {
        var tokenResponse = await tokenService.ValidateToken(request.token);
        if (tokenResponse.IsFailure)
        {
            return tokenResponse.Error;
        }

        var token = tokenResponse.Value;
        var permissions = await userContext.UserPermissions.Where(x => x.UserId == token.UserId)
            .Select(x => x.PermissionId).ToListAsync();
        var roles = await userContext.UserPermissions.Where(x => x.UserId == token.UserId).Select(x => x.PermissionId)
            .ToListAsync();
        return new SignedInUserResponse(token.UserId, token.User.UserName, token.User.Email, token.TokenValue,
            token.ExpiresOn, roles, permissions);
    }

    public async Task<Result<UserListResponse>> GetUserList(UserListRequest request)
    {
        var user = await userContext.Users.Include(x => x.UserRoles).Skip((request.page - 1) * request.rows)
            .Where(x => request.activeUsers ? x.ArchivedOn == null : x.ArchivedOn != null)
            .Select(x => new ListUser(x.Id, x.Name, x.Email, x.UserRoles.Select(y => y.Role.Name).ToList()))
            .Take(request.rows).ToListAsync();
        return new UserListResponse(user, request.rows, request.page);
    }

    public async Task<Result<UserDetailResponse>> GetUser(UserDetailRequest request)
    {
        var user = await userContext.Users.Select(x => new UserDetailResponse(x.Id, x.UserName, x.Email,
            x.UserRoles.Select(y => y.RoleId).ToList(), x.UserPermissions.Select(y => y.PermissionId).ToList()))
            .FirstOrDefaultAsync(x => x.UserId == request.UserId);

        if (user == null)
        {
            return UserErrors.UserNotFound;
        }

        return user;
    }

    public async Task<Result<bool>> InitialPasswordSet(PasswordCreationRequest request)
    {
        var validationResult = userValidator.ValidateUserPasswordCreation(request);
        if (validationResult.IsFailure)
        {
            return validationResult.Error;
        }
        
        var tokenResponse = await tokenService.ValidateToken(request.token);
        if (tokenResponse.IsFailure)
        {
            return tokenResponse.Error;
        }

        var user = tokenResponse.Value.User;
        user.PasswordHash = PasswordHasher.HashPassword(request.password);
        userContext.Users.Update(user);
        await userContext.SaveChangesAsync();
        
        var invalidateResponse = await tokenService.InvalidateToken(request.token);
        if (invalidateResponse.IsFailure)
        {
            return invalidateResponse.Error;
        }

        return true;
    }

    public async Task<Result<bool>> UpdatePassword(UpdatePasswordRequest request)
    {
        var validationResult = userValidator.ValidateUserPasswordUpdate(request);
        if (validationResult.IsFailure)
        {
            return validationResult.Error;
        }
        
        var tokenResponse = await tokenService.ValidateToken(request.token);
        if (tokenResponse.IsFailure)
        {
            return tokenResponse.Error;
        }

        var user = tokenResponse.Value.User;
        user.PasswordHash = PasswordHasher.HashPassword(request.newPassword);
        userContext.Users.Update(user);
        await userContext.SaveChangesAsync();
        
        var invalidateResponse = await tokenService.InvalidateToken(request.token);
        if (invalidateResponse.IsFailure)
        {
            return invalidateResponse.Error;
        }

        return true;
    }

    public async Task<Result<bool>> ResetPassword(ResetPasswordRequest request)
    {
        var user = await userContext.Users.FirstOrDefaultAsync(x => x.Email == request.email);
        
        if (user == null)
        {
            return UserErrors.UserNotFound;
        }
        
        var invalidateAllTokens = await tokenService.InvalidateAllTokens(user.Id);
        if (invalidateAllTokens.IsFailure)
        {
            return invalidateAllTokens.Error;
        }
        
        var newToken = await tokenService.GenerateToken(user.Id);
        if (newToken.IsFailure)
        {
            return newToken.Error;
        }
        
        string htmlTemplate = LoadTemplate.LoadEmbeddedTemplate("PasswordReset.html");

        string localizedBody = htmlTemplate
            .Replace("{ResetLink}", "https://yourdomain.co.za/auth/reset-password?token=xyz123");

        await emailService.SendEmailAsync(user.Email, "Reset Your Password", localizedBody);
        
        return true;
    }

    public async Task<Result<bool>> ArchiveUser(ArchiveUserRequest request, Guid requester)
    {
        var user = await userContext.Users.FirstOrDefaultAsync(x => x.Id == request.userId);
        
        if (user is null)
        {
            return UserErrors.UserNotFound;
        }
        
        var invalidateAllTokens = await tokenService.InvalidateAllTokens(user.Id);
        if (invalidateAllTokens.IsFailure)
        {
            return invalidateAllTokens.Error;
        }
        
        user.ArchivedOn = DateTime.UtcNow;
        user.ArchivedById = requester;
        userContext.Users.Update(user);
        await userContext.SaveChangesAsync();

        return true;
    }

    public async Task<Result<bool>> DeleteUser(DeleteUserRequest request)
    {
        var user = await userContext.Users
            .Where(x => x.ArchivedOn != null)
            .FirstOrDefaultAsync(x => x.Id == request.userId);
        
        if (user is null)
        {
            return UserErrors.UserNotFound;
        }
        
        var invalidateAllTokens = await tokenService.InvalidateAllTokens(user.Id);
        if (invalidateAllTokens.IsFailure)
        {
            return invalidateAllTokens.Error;
        }
        
        userContext.Users.Remove(user);
        await userContext.SaveChangesAsync();

        return true;
    }
}