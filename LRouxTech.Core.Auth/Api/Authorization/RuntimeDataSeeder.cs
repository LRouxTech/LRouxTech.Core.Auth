using System.Reflection;
using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.Auth.Infrastructure.Helper;
using Microsoft.EntityFrameworkCore;

namespace LRouxTech.Core.Auth.Api.Authorization;

public static class RuntimeDataSeeder
{
    public static async Task SeedAdminUserAsync(UserContext context)
    {
        const string adminEmail = "admin@company.com";

        var existingUser = await context.Users
            .FirstOrDefaultAsync(u => u.Email == adminEmail);

        if (existingUser == null)
        {
            var hashedPassword = PasswordHasher.HashPassword("SuperSecretPassword123!");

            var adminUser = new User
            {
                Name = "Admin",
                Surname = "Admin",
                Email = adminEmail,
                UserName = "admin",
                PasswordHash = hashedPassword,
            }.Create();

            await context.Users
                .AddAsync(adminUser);
            await context.SaveChangesAsync();

            var adminRole = await context.Roles
                .FirstAsync(r => r.Name == "Admin");

            var userRoleLink = new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            };

            await context.UserRoles
                .AddAsync(userRoleLink);
            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedRolesAsync<TRoles>(UserContext context)
        where TRoles : AppRoles
    {
        var roleNames = typeof(TRoles)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
            .Select(f => f.GetValue(null)
                ?.ToString())
            .Where(name => !string.IsNullOrEmpty(name))
            .ToList();

        var existingRoles = await context.Roles
            .Where(r => roleNames.Contains(r.Name))
            .ToDictionaryAsync(r => r.Name);

        foreach (var roleName in roleNames)
        {
            if (!existingRoles.ContainsKey(roleName!))
            {
                var newRole = new Role
                {
                    Name = roleName!,
                    Description = $"{roleName} role."
                };
                await context.Roles
                    .AddAsync(newRole);
                existingRoles.Add(roleName!,
                    newRole); 
            }
        }

        await context.SaveChangesAsync();

        if (existingRoles.TryGetValue(UserManagementRoles.Admin,
                out var adminRole))
        {
            var allPermissionsInDb = await context.Permissions
                .ToListAsync();

            var existingRolePermissions = await context.RolePermissions
                .Where(rp => rp.RoleId == adminRole.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            var newAdminLinks = allPermissionsInDb
                .Where(p => !existingRolePermissions.Contains(p.Id))
                .Select(p => new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = p.Id
                })
                .ToList();

            if (newAdminLinks.Any())
            {
                await context.RolePermissions
                    .AddRangeAsync(newAdminLinks);
                await context.SaveChangesAsync();
            }
        }
    }

    public static async Task SeedPermissionsAsync<TPermissions>(UserContext context)
        where TPermissions : AppPermissions
    {
        var permissionKeys = ExtractPermissionKeys(typeof(TPermissions));

        var existingPermissionNames = await context.Permissions
            .Select(p => p.PermissionName)
            .ToListAsync();

        var newPermissions = new List<Permission>();
        foreach (var key in permissionKeys)
        {
            if (!existingPermissionNames.Contains(key.Value))
            {
                newPermissions.Add(new Permission
                {
                    Section = key.Section,
                    PermissionName = key.Value,
                    Description = $"Allows {key.Name} operations in {key.Section}."
                });
            }
        }

        if (newPermissions.Any())
        {
            await context.Permissions
                .AddRangeAsync(newPermissions);
            await context.SaveChangesAsync();
        }
    }

    private static List<PermissionKey> ExtractPermissionKeys(Type type)
    {
        var keys = new List<PermissionKey>();
        var nestedTypes = type.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);
        foreach (var nested in nestedTypes)
        {
            var fields = nested.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(PermissionKey));

            foreach (var field in fields)
            {
                keys.Add((PermissionKey)field.GetValue(null)!);
            }
        }

        return keys;
    }
    
    public static async Task SyncRolePermissionsAsync(
        DbContext context, 
        string roleName, 
        List<string> requiredPermissionNames)
    {
        var role = await context.Set<Role>().FirstOrDefaultAsync(r => r.Name == roleName);
        if (role == null) return; // Or throw an exception if you want to know a role is missing

        var requestedPermissionsInDb = await context.Set<Permission>()
            .Where(p => requiredPermissionNames.Contains(p.PermissionName))
            .ToListAsync();

        var currentRolePermissions = await context.Set<RolePermission>()
            .Include(rp => rp.Permission)
            .Where(rp => rp.RoleId == role.Id)
            .ToListAsync();

        var currentPermissionIds = currentRolePermissions.Select(rp => rp.PermissionId).ToList();
        
        var linksToAdd = requestedPermissionsInDb
            .Where(p => !currentPermissionIds.Contains(p.Id))
            .Select(p => new RolePermission
            {
                RoleId = role.Id,
                PermissionId = p.Id
            })
            .ToList();

        if (linksToAdd.Any())
        {
            await context.Set<RolePermission>().AddRangeAsync(linksToAdd);
        }

        var requestedPermissionIds = requestedPermissionsInDb.Select(p => p.Id).ToList();
        
        var linksToRemove = currentRolePermissions
            .Where(rp => !requestedPermissionIds.Contains(rp.PermissionId))
            .ToList();

        if (linksToRemove.Any())
        {
            context.Set<RolePermission>().RemoveRange(linksToRemove);
        }

        if (linksToAdd.Any() || linksToRemove.Any())
        {
            await context.SaveChangesAsync();
        }
    }
}