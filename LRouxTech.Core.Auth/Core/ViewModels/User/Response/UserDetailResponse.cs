namespace LRouxTech.Core.Auth.Core.ViewModels.User.Response;

public record UserDetailResponse(
    Guid UserId,
    string UserName,
    string Email,
    List<Guid> Roles,
    List<Guid> Permissions);