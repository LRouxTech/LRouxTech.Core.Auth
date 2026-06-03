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
        
        builder.HasOne(up => up.Role)
            .WithMany(u => u.RolePermissions)
            .HasForeignKey(up => up.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(up => up.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(up => up.PermissionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        return builder;
    }
}