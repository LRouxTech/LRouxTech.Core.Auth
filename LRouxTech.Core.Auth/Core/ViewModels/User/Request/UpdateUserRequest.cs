namespace LRouxTech.Core.Auth.Core.ViewModels.User.Request;

public record UpdateUserRequest(
    Guid UserId,
    string Username,
    string Name,
    string Surname,
    string Email,
    List<Guid> RoleIds,
    List<Guid> PermissionIds
    );