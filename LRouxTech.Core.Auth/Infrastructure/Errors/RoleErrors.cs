using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.Auth.Infrastructure.Errors;

public static class RoleErrors
{
    public static readonly Error NoRole = new(
        "Role.NoRole",
        "User needs to have at least 1 role.");
}