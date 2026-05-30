using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.Auth.Infrastructure.Errors;
using LRouxTech.Core.ValidationResult;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LRouxTech.Core.Auth.Infrastructure.Services;

public class TokenService(UserContext userContext, IConfiguration configuration) : ITokenService
{
    public async Task<Result<UserToken>> GenerateToken(Guid UserId)
    {
        string secretKey = configuration["JwtSettings:SecretKey"];
        string issuer = configuration["JwtSettings:Issuer"];
        string audience = configuration["JwtSettings:Audience"];
        
        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            return SettingsErrors.SettingsNotFound;
        }
        
        var user = await userContext.Users
            .Include(x => x.UserRoles)
            .ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == UserId);
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        foreach (var role in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Role.Name));
        }

        // 4. Create the token descriptor
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(14),
            signingCredentials: credentials);
        
        var userToken = new UserToken
        {
            ExpiresOn = DateTime.UtcNow.AddDays(14),
            Expired = false,
            TokenValue = new JwtSecurityTokenHandler().WriteToken(token),
            UserId = user.Id,
        };
        
        userContext.UserTokens.Add(userToken);
        await userContext.SaveChangesAsync();
        
        return userToken;
    }

    public async Task<Result<UserToken>> GetToken(Guid UserId)
    {
        var token = await userContext.UserTokens.FirstOrDefaultAsync(x => x.UserId == UserId);

        if (token == null)
        {
            return await GenerateToken(UserId);
        }
        
        return token;
    }

    public async Task<Result<UserToken>> ValidateToken(string token)
    {
        var userToken = await userContext.UserTokens
            .Include(x => x.User)
            .Where(x => !x.Expired)
            .FirstOrDefaultAsync(x => x.TokenValue == token);

        if (userToken == null)
        {
            return TokenErrors.TokenNotFound;
        }

        if (userToken.ExpiresOn <= DateTime.Now)
        {
            userToken.Expired = true;
            userContext.UserTokens.Update(userToken);
            await userContext.SaveChangesAsync();
            return TokenErrors.TokenExpired;
        }
        
        return userToken;
    }

    public async Task<Result<bool>> InvalidateToken(string token)
    {
        var userToken = await userContext.UserTokens.FirstOrDefaultAsync(x => x.TokenValue == token);
        if (userToken == null)
        {
            return true;
        }
        userToken.Expired = true;
        userContext.UserTokens.Update(userToken);
        await userContext.SaveChangesAsync();
        return true;
    }

    public async Task<Result<bool>> InvalidateAllTokens(Guid userId)
    {
        var userToken = await userContext.UserTokens.Where(x => x.UserId == userId).ToListAsync();
        if (userToken is null or [])
        {
            return true;
        }

        foreach (var token in userToken)
        {
            token.Expired = true;
            userContext.UserTokens.Update(token);
        }
        await userContext.SaveChangesAsync();
        return true;
    }
}