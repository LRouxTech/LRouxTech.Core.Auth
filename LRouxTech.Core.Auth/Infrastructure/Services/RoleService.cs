using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Core.ViewModels.Role;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.Auth.Infrastructure.Errors;
using LRouxTech.Core.ValidationResult;
using Microsoft.EntityFrameworkCore;

namespace LRouxTech.Core.Auth.Infrastructure.Services;

public class RoleService(UserContext userContext):IRoleService
{
    public async Task<Result<RoleListResponse>> GetList()
    {
        var roles = await userContext.Roles
            .Include(x => x.RolePermissions)
            .Select(x => new RoleItem(
                x.Id,
                x.Name,
                x.Description,
                x.RolePermissions.Select(y => y.PermissionId).ToList()
            ))
            .ToListAsync();

        if (roles is null or [])
        {
            return RoleErrors.NoRolesFound;
        }

        return new RoleListResponse(roles);
    }
}