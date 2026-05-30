using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Core.ViewModels.User;
using LRouxTech.Core.Auth.Core.ViewModels.User.Reponse;
using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.Auth.Infrastructure.Errors;
using LRouxTech.Core.Auth.Infrastructure.Helper;
using LRouxTech.Core.Auth.Infrastructure.Validator;
using LRouxTech.Core.ValidationResult;
using Microsoft.EntityFrameworkCore;

namespace LRouxTech.Core.Auth.Infrastructure.Services;

public class UserService(UserContext userContext, ITokenService tokenService, IUserValidator userValidator)
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
        return user;
    }

    public async Task<Result<User>> Update(UpdateUserRequest request)
    {
        var validationResult = userValidator.ValidateUserUpdate(request);
        if (validationResult.IsFailure)
        {
            return validationResult.Error;
        }
        
        var user = await userContext.Users.Include(x => x.UserPermissions).Include(x => x.UserRoles).FirstOrDefaultAsync(x => x.Id == request.UserId);
        if (user == null)
        {
            return UserErrors.UserNotFound;
        }
        user.Name = request.Name;
        user.Email = request.Email;
        user.UserName = request.Username;
        
        var rolesToRemove = user.UserRoles
            .Where(ur => !request.RoleIds.Contains(ur.RoleId))
            .ToList();

        foreach (var role in rolesToRemove)
        {
            user.UserRoles.Remove(role);
        }

        var existingRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToHashSet();
        var rolesToAdd = request.RoleIds
            .Where(id => !existingRoleIds.Contains(id))
            .Select(id => new UserRole { UserId = user.Id, RoleId = id });

        foreach (var newRole in rolesToAdd)
        {
            user.UserRoles.Add(newRole);
        }
        
        var permissionsToRemove = user.UserPermissions
            .Where(up => !request.PermissionIds.Contains(up.PermissionId))
            .ToList();

        foreach (var permission in permissionsToRemove)
        {
            user.UserPermissions.Remove(permission);
        }

        var existingPermissionIds = user.UserPermissions.Select(up => up.PermissionId).ToHashSet();
        var permissionsToAdd = request.PermissionIds
            .Where(id => !existingPermissionIds.Contains(id))
            .Select(id => new UserPermission { UserId = user.Id, PermissionId = id });

        foreach (var newPermission in permissionsToAdd)
        {
            user.UserPermissions.Add(newPermission);
        }
        userContext.Users.Update(user);
        await userContext.SaveChangesAsync();
        return user;
    }

    public Task<Result<SignedInUserResponse>> Authenticate(AuthenticateUserRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserListResponse>> GetUserList(UserListRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<UserDetailResponse>> GetUser(UserDetailRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> InitialPasswordSet(PasswordCreationRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> UpdatePassword(UpdatePasswordRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> ResetPassword(ResetPasswordRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> ArchiveUser(ArchiveUserRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> DeleteUser(DeleteUserRequest request)
    {
        throw new NotImplementedException();
    }
}