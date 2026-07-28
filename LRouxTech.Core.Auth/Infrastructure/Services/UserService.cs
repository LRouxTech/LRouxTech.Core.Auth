using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Core.ViewModels.User;
using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using LRouxTech.Core.Auth.Core.ViewModels.User.Response;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.Auth.Infrastructure.Errors;
using LRouxTech.Core.Auth.Infrastructure.Helper;
using LRouxTech.Core.Auth.Infrastructure.Paged;
using LRouxTech.Core.Auth.Infrastructure.Templates;
using LRouxTech.Core.Auth.Infrastructure.Validator;
using LRouxTech.Core.Mail;
using LRouxTech.Core.ValidationResult;
using Microsoft.EntityFrameworkCore;

namespace LRouxTech.Core.Auth.Infrastructure.Services;

public class UserService(IUserDbContextFactory dbContextFactory, ITokenService tokenService, IUserValidator userValidator, IEmailService emailService)
    : IUserService
{
    public async Task<Result<SignedInUserResponse>> Login(UserLoginRequest request)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.Users
            .Include(x => x.UserTokens.Where(x => !x.Expired && x.ExpiresOn > DateTime.UtcNow))
            .Include(x => x.UserPermissions)
            .Include(x => x.UserRoles).ThenInclude(x => x.Role).ThenInclude(x => x.RolePermissions)
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
            user.UserRoles.Select(x => x.RoleId).ToList(), user.UserPermissions.Select(x => x.PermissionId).Concat(user.UserRoles.SelectMany(x => x.Role.RolePermissions).Select(x => x.PermissionId)).Distinct().ToList());
    }

    public async Task<Result<bool>> Logout(UserLogoutRequest request)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.Users
            .Include(x => x.UserTokens.Where(x => !x.Expired && x.ExpiresOn > DateTime.UtcNow))
            .FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (user == null)
        {
            return UserErrors.UserNotFound;
        }

        foreach (var token in user.UserTokens)
        {
            token.Expired = true;
            dbContext.UserTokens.Update(token);
        }

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Result<User>> Create(CreateUserRequest request)
    { 
        var validationResult = userValidator.ValidateUserCreation(request);
        if (validationResult.IsFailure)
        {
            return validationResult.Error;
        }
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var tempHash = new byte[32];
        System.Security.Cryptography.RandomNumberGenerator.Fill(tempHash);

        var user = new User
        {
            Name = request.Name,
            Surname = request.Surname,
            Email = request.Email,
            UserName = request.Username,
            PasswordHash = tempHash,
            UserRoles = request.RoleIds.Select(x => new UserRole { RoleId = x }).ToList(),
            UserPermissions = request.PermissionIds.Select(x => new UserPermission { PermissionId = x }).ToList(),
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        
        user = await dbContext.Users
            .Include(x => x.UserPermissions)
            .Include(x => x.UserRoles)
            .FirstOrDefaultAsync(x => x.Id == user.Id);

        if (user == null)
        {
            return UserErrors.UserNotFound;
        }

        var tokenResult = await tokenService.GenerateToken(user.Id);
        if (tokenResult.IsFailure)
        {
            return tokenResult.Error;
        }
        
        // Replace with appsettings
        string domain = "";
        var placeholders = new Dictionary<string, string>
        {
            { "ResetLink", $"https://{domain}/auth/reset-password?token={tokenResult.Value.TokenValue}" },
        };
        
        string htmlTemplate = LoadTemplate.RenderTemplate("WelcomeMail.html", placeholders);
        
        await emailService.SendEmailAsync(user.Email, "Reset Your Password", htmlTemplate);
        

        
        return user;
    }

    public async Task<Result<User>> Update(UpdateUserRequest request)
    {
        var validationResult = userValidator.ValidateUserUpdate(request);
        if (validationResult.IsFailure)
        {
            return validationResult.Error;
        }
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var user = await dbContext.Users.Include(x => x.UserPermissions).Include(x => x.UserRoles)
            .FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (user == null)
        {
            return UserErrors.UserNotFound;
        }

        user.Name = request.Name;
        user.Surname = request.Surname;
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

        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<Result<SignedInUserResponse>> Authenticate(AuthenticateUserRequest request)
    {
        var tokenResponse = await tokenService.ValidateToken(request.token);
        if (tokenResponse.IsFailure)
        {
            return tokenResponse.Error;
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var token = tokenResponse.Value;
        var permissions = await dbContext.UserPermissions.Where(x => x.UserId == token.UserId)
            .Select(x => x.PermissionId).ToListAsync();
        var roles = await dbContext.UserPermissions.Where(x => x.UserId == token.UserId).Select(x => x.PermissionId)
            .ToListAsync();
        return new SignedInUserResponse(token.UserId, token.User.UserName, token.User.Email, token.TokenValue,
            token.ExpiresOn, roles, permissions);
    }

    public async Task<Result<PagedList<ListUser>>> GetUserList(PagedRequest request)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var query = dbContext.Users.AsNoTracking();

        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderBy(x => x.Id)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new ListUser(x.Id, x.Name, x.Email, x.UserRoles.Select(y => y.Role.Name).ToList()))
            .ToListAsync();
        
        return new PagedList<ListUser>(items, totalCount, request.PageIndex, request.PageSize);
    }

    public async Task<Result<UserDetailResponse>> GetUser(UserDetailRequest request)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.Users.Select(x => new {x.Id, x.UserName, x.Email,
            UserRoles = x.UserRoles.Select(y => y.RoleId), Permissions = x.UserPermissions.Select(y => y.PermissionId)})
            .FirstOrDefaultAsync(x => x.Id == request.UserId);

        if (user == null)
        {
            return UserErrors.UserNotFound;
        }

        return  new UserDetailResponse(
            user.Id, 
            user.UserName, 
            user.Email,
            user.UserRoles.ToList(),
            user.Permissions.ToList()
        );
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

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = tokenResponse.Value.User;
        user.PasswordHash = PasswordHasher.HashPassword(request.password);
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();
        
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

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = tokenResponse.Value.User;
        user.PasswordHash = PasswordHasher.HashPassword(request.newPassword);
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();
        
        var invalidateResponse = await tokenService.InvalidateToken(request.token);
        if (invalidateResponse.IsFailure)
        {
            return invalidateResponse.Error;
        }

        return true;
    }

    public async Task<Result<bool>> ResetPassword(ResetPasswordRequest request)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == request.email);
        
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

        // Replace with appsettings
        string domain = "";
        
        var placeholders = new Dictionary<string, string>
        {
            { "ResetLink", $"https://{domain}/auth/reset-password?token={newToken}" },
        };
        
        string htmlTemplate = LoadTemplate.RenderTemplate("ResetPassword.html", placeholders);

        await emailService.SendEmailAsync(user.Email, "Reset Your Password", htmlTemplate);
        
        return true;
    }

    public async Task<Result<bool>> ArchiveUser(ArchiveUserRequest request, Guid requester)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.userId);
        
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
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<Result<bool>> DeleteUser(DeleteUserRequest request)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = await dbContext.Users
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
        
        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();

        return true;
    }
}