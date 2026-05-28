using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.Auth.Infrastructure.Database.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

namespace LRouxTech.Core.Auth.Infrastructure.Database;

public class UserDbContextFactory : IDesignTimeDbContextFactory<UserContext>
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
                x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Auth");
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
    }
    
    public UserContext()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var optionsBuilder = new DbContextOptionsBuilder<UserContext>();
        
        optionsBuilder.UseNpgsql(connectionString, x =>
        {
            x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "user");
            x.MigrationsAssembly("LRouxTech.Core.Auth");
        });
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        
            var connectionString = configuration.GetConnectionString("DefaultConnection");
        
            optionsBuilder.UseNpgsql(connectionString, x =>
            {
                x.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "user");
                x.MigrationsAssembly("LRouxTech.Core.Auth");
            });
        }
        
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        optionsBuilder.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Permission>().ConfigurePermission();
        modelBuilder.Entity<Role>().ConfigureRole();
        modelBuilder.Entity<RolePermission>().ConfigureRolePermission();
        modelBuilder.Entity<User>().ConfigureUser();
        modelBuilder.Entity<UserPermission>().ConfigureUserPermission();
        modelBuilder.Entity<UserRole>().ConfigureUserRole();
        modelBuilder.Entity<UserToken>().ConfigureUserToken();
    }
}