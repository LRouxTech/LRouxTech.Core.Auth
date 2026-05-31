using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.Auth.Core.ViewModels.User;
using LRouxTech.Core.Auth.Core.ViewModels.User.Reponse;
using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.Auth.Core.Interfaces;

public interface IUserService
{
    Task<Result<SignedInUserResponse>> Login(UserLoginRequest request);
    Task<Result<bool>> Logout(UserLogoutRequest request);
    Task<Result<User>> Create(CreateUserRequest request);
    Task<Result<User>> Update(UpdateUserRequest request);
    Task<Result<SignedInUserResponse>> Authenticate(AuthenticateUserRequest request);
    Task<Result<UserListResponse>> GetUserList(UserListRequest request);
    Task<Result<UserDetailResponse>> GetUser(UserDetailRequest request);
    Task<Result<bool>> InitialPasswordSet(PasswordCreationRequest request);
    Task<Result<bool>> UpdatePassword(UpdatePasswordRequest request);
    Task<Result<bool>> ResetPassword(ResetPasswordRequest request);
    Task<Result<bool>> ArchiveUser(ArchiveUserRequest request, Guid requester);
    Task<Result<bool>> DeleteUser(DeleteUserRequest request);
}