using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.Auth.Infrastructure.Errors;
using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.Auth.Infrastructure.Validator;

public class UserValidator : IUserValidator
{
    public Result<bool> ValidateUserCreation(CreateUserRequest model)
    {
        if (string.IsNullOrWhiteSpace(model.Username))
        {
            return UserErrors.NoUsername;
        }

        if (string.IsNullOrWhiteSpace(model.Email))
        {
            return UserErrors.NoEmail;
        }

        if (model.RoleIds is null or [])
        {
            return RoleErrors.NoRole;
        }

        return true;
    }

    public Result<bool> ValidateUserUpdate(UpdateUserRequest model)
    {
        if (model.UserId == Guid.Empty)
        {
            return UserErrors.NoUserId;
        }
        
        if (string.IsNullOrWhiteSpace(model.Username))
        {
            return UserErrors.NoUsername;
        }

        if (string.IsNullOrWhiteSpace(model.Email))
        {
            return UserErrors.NoEmail;
        }

        if (model.RoleIds is null or [])
        {
            return RoleErrors.NoRole;
        }

        return true;
    }

    public Result<bool> ValidateUserPasswordCreation(PasswordCreationRequest model)
    {
        if (string.IsNullOrWhiteSpace(model.token))
        {
            return TokenErrors.EmptyToken;
        }
        
        if (string.IsNullOrWhiteSpace(model.password))
        {
            return PasswordErrors.EmptyPassword;
        }
        
        if (string.IsNullOrWhiteSpace(model.passwordConfirmation))
        {
            return PasswordErrors.EmptyConfirmPassword;
        }

        if(!string.Equals(model.password,model.passwordConfirmation, StringComparison.CurrentCulture))
        {
            return PasswordErrors.PasswordsdontMatch;
        }

        if (model.password.Length < 8)
        {
            return PasswordErrors.TooShort;
        }

        if (!model.password.Any(char.IsUpper))
        {
            return PasswordErrors.NoUppercase;
        }

        if (!model.password.Any(char.IsLower))
        {
            return PasswordErrors.NoLowercase;
        }

        if (!model.password.Any(char.IsNumber))
        {
            return PasswordErrors.NoNumber;
        }

        return true;
    }

    public Result<bool> ValidateUserPasswordUpdate(UpdatePasswordRequest model)
    {
        if (string.IsNullOrWhiteSpace(model.token))
        {
            return TokenErrors.EmptyToken;
        }
        
        if (string.IsNullOrWhiteSpace(model.newPassword))
        {
            return PasswordErrors.EmptyPassword;
        }
        
        if (string.IsNullOrWhiteSpace(model.newPasswordConfirm))
        {
            return PasswordErrors.EmptyConfirmPassword;
        }

        if(!string.Equals(model.newPassword,model.newPasswordConfirm, StringComparison.CurrentCulture))
        {
            return PasswordErrors.PasswordsdontMatch;
        }

        if (model.newPassword.Length < 8)
        {
            return PasswordErrors.TooShort;
        }

        if (!model.newPassword.Any(char.IsUpper))
        {
            return PasswordErrors.NoUppercase;
        }

        if (!model.newPassword.Any(char.IsLower))
        {
            return PasswordErrors.NoLowercase;
        }

        if (!model.newPassword.Any(char.IsNumber))
        {
            return PasswordErrors.NoNumber;
        }

        return true;
    }
}
