namespace LRouxTech.Core.Auth.Core.ViewModels;

public record CreateUserRequest(
    string Username,
    string Email,
    string Password
);