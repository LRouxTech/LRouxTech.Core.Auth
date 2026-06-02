using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Core.ViewModels.Permission;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.Auth.Infrastructure.Errors;
using LRouxTech.Core.ValidationResult;
using Microsoft.EntityFrameworkCore;

namespace LRouxTech.Core.Auth.Infrastructure.Services;

public class PermissionService(IUserDbContextFactory dbContextFactory) : IPermissionService
{
    public async Task<Result<PermissionListResponse>> GetList()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var permissions = await dbContext.Permissions
            .Select(x => new PermissionItem(x.Id, x.Section, x.PermissionName))
            .ToListAsync();

        if (permissions is null or [])
        {
            return PermissionErrors.NoPermissions;
        }
        
        return new PermissionListResponse(permissions);
    }
}