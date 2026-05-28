namespace LRouxTech.Core.Auth.Core.ViewModels.User.Request;

public record UpdateUserRequest(
    Guid UserId,
    string Username,
    string Email,
    List<Guid> RoleIds,
    List<Guid> PermissionIds
    );