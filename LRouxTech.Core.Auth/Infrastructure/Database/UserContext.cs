using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.Auth.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

namespace LRouxTech.Core.Auth.Infrastructure.Database;

public class UserDbContextFactory
{
    public UserContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UserContext>();
        var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", false, true)
            .AddEnvironmentVariables();
        var configuration = builder.Build();
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), x =>
            {
                x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "user");
                x.MigrationsAssembly("LRouxTech.Core.Auth");
            });
        }

        return new UserContext(optionsBuilder.Options);
    }
}

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
        Database.SetCommandTimeout((int)TimeSpan.FromMinutes(1).TotalSeconds);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }
}