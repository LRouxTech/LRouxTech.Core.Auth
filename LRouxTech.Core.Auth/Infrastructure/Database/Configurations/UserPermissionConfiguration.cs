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

        builder.HasOne<User>(x => x.User)
            .WithMany(x => x.UserPermissions)
            .HasForeignKey(x => x.UserId)
            .IsRequired();
        
        builder.HasOne<Permission>(x => x.Permission)
            .WithMany(x => x.UserPermissions)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        return builder;
    }
}