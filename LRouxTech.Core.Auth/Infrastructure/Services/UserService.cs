using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Core.ViewModels.User;
using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using LRouxTech.Core.Auth.Infrastructure.Database;

namespace LRouxTech.Core.Auth.Infrastructure.Services;

public class UserService(UserContext userContext) : IUserService
{
    public Task<User> Login(UserLoginRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<User> Create(CreateUserRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<User> Authenticate(AuthenticateUserRequest request)
    {
        throw new NotImplementedException();
    }
}