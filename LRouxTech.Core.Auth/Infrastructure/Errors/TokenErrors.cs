using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.Auth.Infrastructure.Errors;

public static class TokenErrors
{
    public static readonly Error EmptyToken = new(
        "Token.EmtpyToken",
        "Token is empty.");
    
    public static readonly Error TokenNotFound = new(
        "Token.NotFound",
        "No token is found.");
    
    public static readonly Error TokenExpired = new(
        "Token.Expired",
        "Token has expired.");
}