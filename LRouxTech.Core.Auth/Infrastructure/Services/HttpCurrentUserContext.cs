using System.Security.Claims;
using LRouxTech.Core.Auth.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LRouxTech.Core.Auth.Infrastructure.Services;

public class HttpCurrentUserContext(IHttpContextAccessor httpContextAccessor) : IHttpCurrentUserContext
{
    public Guid? UserId
    {
        get
        {
            var claimValue = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

            return Guid.TryParse(claimValue, out var parsedGuid) ? parsedGuid : null;
        }
    }
}