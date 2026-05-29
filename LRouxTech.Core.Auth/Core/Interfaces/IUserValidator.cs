using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.Auth.Core.Interfaces;

public interface IUserValidator
{
   Result ValidateUserCreation(CreateUserRequest model);
   Result ValidateUserUpdate(UpdateUserRequest model);
   Result ValidateUserPasswordCreation(PasswordCreationRequest model);
   Result ValidateUserPasswordUpdate(UpdatePasswordRequest model);
}