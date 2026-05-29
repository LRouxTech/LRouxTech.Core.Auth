using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.Auth.Core.Interfaces;

public interface IUserValidator
{
   Result<bool> ValidateUserCreation(CreateUserRequest model);
   Result<bool> ValidateUserUpdate(UpdateUserRequest model);
   Result<bool> ValidateUserPasswordCreation(PasswordCreationRequest model);
   Result<bool> ValidateUserPasswordUpdate(UpdatePasswordRequest model);
}