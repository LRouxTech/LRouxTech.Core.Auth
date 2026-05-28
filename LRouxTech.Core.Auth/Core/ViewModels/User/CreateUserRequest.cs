namespace LRouxTech.Core.Auth.Core.ViewModels.User;

public record CreateUserRequest(
    string Username,
    string Email,
    string Password,
    Guid RoleId,
    List<Guid> PermissionIds
);