using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.Auth.Infrastructure.Errors;

public static class TokenErrors
{
    public static readonly Error EmptyToken = new(
        "Token.EmtpyToken",
        "Token is empty.");
}