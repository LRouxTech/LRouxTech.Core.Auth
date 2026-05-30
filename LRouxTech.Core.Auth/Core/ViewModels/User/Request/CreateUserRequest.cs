namespace LRouxTech.Core.Auth.Core.ViewModels.User.Request;

public record CreateUserRequest(
    string Name,
    string Surname,
    string Username,
    string Email,
    List<Guid> RoleIds,
    List<Guid> PermissionIds
);