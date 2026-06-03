using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.BaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LRouxTech.Core.Auth.Infrastructure.Database.Configurations;

public static class UserTokenConfiguration
{
    public static EntityTypeBuilder<UserToken> ConfigureUserToken(this EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable("UserTokens");

        builder.HasKey(x => x.UserTokenId);
        
        builder.Property(u => u.UserId)
            .IsRequired();

        builder.Property(u => u.TokenValue)
            .IsRequired()
            .HasMaxLength(2048);
        
        builder.Property(u => u.CreatedOn)
            .IsRequired();
        
        builder.Property(u => u.ExpiresOn)
            .IsRequired();
        
        builder.Property(u => u.Expired)
            .IsRequired();

        builder.HasOne<User>(x => x.User)
            .WithMany(x => x.UserTokens)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        return builder;
    }
}