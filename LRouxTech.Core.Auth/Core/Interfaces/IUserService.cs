using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.Auth.Core.ViewModels.User;
using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using LRouxTech.Core.Auth.Core.ViewModels.User.Response;
using LRouxTech.Core.Auth.Infrastructure.Paged;
using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.Auth.Core.Interfaces;

public interface IUserService
{
    Task<Result<SignedInUserResponse>> Login(UserLoginRequest request);
    Task<Result<bool>> Logout(UserLogoutRequest request);
    Task<Result<UserDetailResponse>> Create(CreateUserRequest request);
    Task<Result<UserDetailResponse>> Update(UpdateUserRequest request);
    Task<Result<SignedInUserResponse>> Authenticate(AuthenticateUserRequest request);
    Task<Result<PagedList<ListUser>>> GetUserList(PagedRequest request);
    Task<Result<UserDetailResponse>> GetUser(UserDetailRequest request);
    Task<Result<bool>> InitialPasswordSet(PasswordCreationRequest request);
    Task<Result<bool>> UpdatePassword(UpdatePasswordRequest request);
    Task<Result<bool>> ResetPassword(ResetPasswordRequest request);
    Task<Result<bool>> ArchiveUser(ArchiveUserRequest request, Guid requester);
    Task<Result<bool>> DeleteUser(DeleteUserRequest request);
}