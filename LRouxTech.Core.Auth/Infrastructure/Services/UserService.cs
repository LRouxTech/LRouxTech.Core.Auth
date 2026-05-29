using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Core.ViewModels.User;
using LRouxTech.Core.Auth.Core.ViewModels.User.Reponse;
using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.Auth.Infrastructure.Services;

public class UserService(UserContext userContext) : IUserService
{
    public Task<Result<User>> Login(UserLoginRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> Logout(UserLogoutRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<User>> Create(CreateUserRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<User>> Update(UpdateUserRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<Result<User>> Authenticate(AuthenticateUserRequest request)
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