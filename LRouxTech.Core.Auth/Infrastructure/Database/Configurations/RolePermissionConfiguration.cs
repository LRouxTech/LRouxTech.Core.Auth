using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LRouxTech.Core.Auth.Infrastructure.Database.Configurations;

public static class RolePermissionConfiguration
{
    public static EntityTypeBuilder<RolePermission> ConfigureRolePermission(this EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");
        builder.ConfigureBaseModel();

        builder.Property(u => u.RoleId)
            .IsRequired();

        builder.Property(u => u.PermissionId)
            .IsRequired();

        builder.HasOne<Role>(x => x.Role)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.RoleId)
            .IsRequired();
        
        builder.HasOne<Permission>(x => x.Permission)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.PermissionId)
            .IsRequired();

        return builder;
    }
}