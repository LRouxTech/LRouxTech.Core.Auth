using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LRouxTech.Core.Auth.Infrastructure.Database.Configurations;

public static class UserRoleConfiguration
{
    public static EntityTypeBuilder<UserRole> ConfigureUserRole(this EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles");
        builder.ConfigureBaseModel();

        builder.Property(u => u.UserId)
            .IsRequired();

        builder.Property(u => u.RoleId)
            .IsRequired();

        builder.HasOne<User>(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId)
            .IsRequired();
        
        builder.HasOne<Role>(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        return builder;
    }
}