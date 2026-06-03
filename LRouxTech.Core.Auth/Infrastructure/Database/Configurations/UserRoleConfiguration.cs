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
        
        builder.HasOne(up => up.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(up => up.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(up => up.Role)
            .WithMany(p => p.UserRoles)
            .HasForeignKey(up => up.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        return builder;
    }
}