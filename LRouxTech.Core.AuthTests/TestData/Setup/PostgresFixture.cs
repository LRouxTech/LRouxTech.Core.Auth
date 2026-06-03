using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.AuthTests.TestData.EntityData;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace LRouxTech.Core.AuthTests.TestData.Setup;

public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;
    public DbContextOptions<UserContext> DbOptions { get; private set; } = null!;

    public PostgresFixture()
    {
        _container = new PostgreSqlBuilder("postgres:16-alpine")
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync();

        DbOptions = new DbContextOptionsBuilder<UserContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        // Run migrations using a temporary context instance
        await using var context = new UserContext(DbOptions);
        await context.Database.MigrateAsync();
        await SeedDefaultDataAsync(context);
    }

    private async Task SeedDefaultDataAsync(UserContext context)
    {
        await UserData.SeedData(context);
        await PermissionData.SeedData(context);
        await RoleData.SeedData(context);
    }

    public async ValueTask DisposeAsync()
    {
        await _container.StopAsync();
    }
}