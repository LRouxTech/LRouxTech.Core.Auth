namespace LRouxTech.Core.Auth.Core.ViewModels.User.Request;

public record CreateUserRequest(
    string Username,
    string Email,
    List<Guid> RoleIds,
    List<Guid> PermissionIds
);