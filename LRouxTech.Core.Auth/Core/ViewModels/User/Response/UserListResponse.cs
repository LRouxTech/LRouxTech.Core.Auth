namespace LRouxTech.Core.Auth.Core.ViewModels.User.Response;

public record UserListResponse(
    List<ListUser> Users,
    int rows = 20,
    int page = 1);

public record ListUser(
    Guid UserId,
    string Username,
    string Email,
    List<String> Roles);

