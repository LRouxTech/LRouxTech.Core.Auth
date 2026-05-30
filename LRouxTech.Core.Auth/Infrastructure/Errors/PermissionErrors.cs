using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.Auth.Infrastructure.Errors;

public static class PermissionErrors
{
    public static readonly Error NoPermissions = new("Permissions.Empty", "No permissions found.");
}