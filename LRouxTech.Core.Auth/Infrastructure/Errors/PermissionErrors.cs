using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.Auth.Infrastructure.Errors;

public static class PermissionErrors
{
    public static readonly Error NoUsername = new("User.EmptyUsername", "Username cannot be empty.");
    public static readonly Error NoEmail = new("User.EmptyEmail", "Email cannot be empty.");

    public static readonly Error NonPublicProfile = new(
        "Followers.NonPublicProfile",
        "Can't follow non-public profiles");
    
    public static readonly Error AlreadyFollowing = new(
        "Followers.AlreadyFollowing",
        "Already following");
}