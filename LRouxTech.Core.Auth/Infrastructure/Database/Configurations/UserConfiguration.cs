using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LRouxTech.Core.Auth.Infrastructure.Database.Configurations;

public static class UserConfiguration
{
    public static EntityTypeBuilder<User> ConfigureUser(this EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.ConfigureBaseModel();

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(u => u.Surname)
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(500);

        builder.HasIndex(u => u.Email)
            .IsUnique();
        
        builder.HasMany<UserPermission>(x => x.UserPermissions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();
        
        builder.HasMany<UserRole>(x => x.UserRoles)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();
        
        builder.HasMany<UserToken>(x => x.UserTokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        return builder;
    }
}