namespace LRouxTech.Core.Auth.Core.ViewModels.User.Reponse;

public record UserDetailResponse(
    Guid UserId,
    string UserName,
    string Email,
    List<Guid> Roles,
    List<Guid> Permissions);