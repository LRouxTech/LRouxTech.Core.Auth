using LRouxTech.Core.Auth.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace LRouxTech.Tests.Auth.TestData.Setup;

public class TestDbContextFactory : IUserDbContextFactory
{
    private readonly DbContextOptions<UserContext> _options;

    public TestDbContextFactory(DbContextOptions<UserContext> options)
    {
        _options = options;
    }

    public UserContext CreateDbContext()
    {
        return new UserContext(_options);
    }
}