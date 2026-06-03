using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LRouxTech.Core.Auth.Infrastructure.Database.Configurations;

public static class UserPermissionConfiguration
{
    public static EntityTypeBuilder<UserPermission> ConfigureUserPermission(this EntityTypeBuilder<UserPermission> builder)
    {
        builder.ToTable("UserPermissions");
        builder.ConfigureBaseModel();

        builder.Property(u => u.UserId)
            .IsRequired();

        builder.Property(u => u.PermissionId)
            .IsRequired();
        
        builder.HasOne(up => up.User)
            .WithMany(u => u.UserPermissions)
            .HasForeignKey(up => up.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(up => up.Permission)
            .WithMany(p => p.UserPermissions)
            .HasForeignKey(up => up.PermissionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        return builder;
    }
}