using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.Auth.Core.Interfaces;

public interface ITokenService
{
    Task<Result<UserToken>> GenerateToken(Guid UserId);
    Task<Result<UserToken>> GetToken(Guid UserId);
    Task<Result<UserToken>> ValidateToken(string token);
    Task<Result<bool>> InvalidateToken(string token);
    Task<Result<bool>> InvalidateAllTokens(Guid userId);
}