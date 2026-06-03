using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LRouxTech.Core.Auth.Infrastructure.Database.Configurations;

public static class RoleConfiguration
{
    public static EntityTypeBuilder<Role> ConfigureRole(this EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.ConfigureBaseModel();

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Description)
            .IsRequired()
            .HasMaxLength(100);

        return builder;
    }
}