using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.Auth.Infrastructure.Errors;

public static class UserErrors
{
    public static readonly Error UserNotFound = new("User.NotFound", "No User found.");
    public static readonly Error NoUserId = new("User.NoUserId", "No UserId.");
    public static readonly Error NoUsername = new("User.EmptyUsername", "Username cannot be empty.");
    public static readonly Error NoEmail = new("User.EmptyEmail", "Email cannot be empty.");
    public static readonly Error InvalidPassword = new("User.InvalidPassword", "Password does not match.");
}