namespace LRouxTech.Core.Auth.Core.ViewModels.User.Request;

public record UserLoginRequest
(
    string UserName,
    string Password
);