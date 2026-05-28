using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.Auth.Core.ViewModels.User;

namespace LRouxTech.Core.Auth.Core.Interfaces;

public interface IUserService {
  Task<User> Login(UserLoginRequest  request);
  Task<User> Create(CreateUserRequest  request);
  Task<User> Authenticate(AuthenticateUserRequest   request);
}