using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LRouxTech.Core.Auth.Infrastructure.Database.Configurations;

public static class PermissionConfiguration
{
    public static EntityTypeBuilder<Permission> ConfigurePermission(this EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");
        builder.ConfigureBaseModel();

        builder.Property(u => u.Section)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.PermissionName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(u => u.Description)
            .HasMaxLength(150);
        
        return builder;
    }
}