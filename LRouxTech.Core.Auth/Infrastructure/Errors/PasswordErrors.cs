using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.Auth.Infrastructure.Errors;

public static class PasswordErrors
{
    public static readonly Error EmptyPassword = new("Password.EmptyPassword", "Password cannot be empty.");
    public static readonly Error EmptyConfirmPassword = new("Password.EmptyConfirmPassword", "Confirm password cannot be empty.");
    public static readonly Error PasswordsdontMatch = new("Password.PasswordsdontMatch", "Passwords don't match.");
    public static readonly Error TooShort= new("Password.TooShort", "Password lenght should be 8 or more characters.");
    public static readonly Error NoUppercase = new("Password.NoUppercase", "Password needs at least 1 uppercase character.");
    public static readonly Error NoLowercase = new("Password.NoLowercase", "Password needs at least 1 lowercase character.");
    public static readonly Error NoNumber = new("Password.NoNumber", "Password needs at least 1 number.");
}
