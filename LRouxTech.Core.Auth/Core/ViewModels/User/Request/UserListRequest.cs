namespace LRouxTech.Core.Auth.Core.ViewModels.User.Request;

public record UserListRequest(bool activeUsers, int rows = 20,
    int page = 1 );