namespace LRouxTech.Core.Auth.Core.ViewModels.User.Response;

public record SignedInUserResponse(
    Guid UserId,
    string UserName,
    string Email,
    string tokenValue,
    DateTime tokenExpiresOn,
    List<Guid> Roles,
    List<Guid> Permissions);