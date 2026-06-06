using LRouxTech.Core.Auth.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace LRouxTech.Core.Auth.Api.Authorization;

public static class DataSeeder
{
    public static void SeedPermissionsAndRoles(ModelBuilder modelBuilder)
    {
        var permissions = new List<Permission>();

        var sections = typeof(AppPermissions).GetNestedTypes();
        foreach (var section in sections)
        {
            var sectionName = section.GetField("Section")?.GetValue(null)?.ToString() ?? section.Name;
            var fields = section.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(f => f.Name != "Section");

            foreach (var field in fields)
            {
                var value = field.GetValue(null)?.ToString();
                permissions.Add(new Permission
                {
                    Id = Guid.CreateVersion7(),
                    Section = sectionName,
                    PermissionName = value,
                    Description = $"Allows {field.Name} operations in {sectionName}."
                });
            }
        }

        modelBuilder.Entity<Permission>().HasData(permissions);

        var adminRole = new Role { Id = Guid.CreateVersion7(), Name = AppRoles.Admin, Description = "Full System Administrator" };
        
        modelBuilder.Entity<Role>().HasData(adminRole);

        var adminPermissions = permissions.Select(p => new RolePermission
        {
            RoleId = adminRole.Id,
            PermissionId = p.Id
        }).ToArray();

        modelBuilder.Entity<RolePermission>().HasData(adminPermissions);
    }
}