using FluentAssertions;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.Auth.Infrastructure.Services;
using LRouxTech.Core.AuthTests.TestData.Setup;

namespace LRouxTech.Core.AuthTests.UnitTests.ServiceTests;

public class RoleServiceTests : IAsyncLifetime
{
    private RoleService _roleService;
    private PostgresFixture _fixture = null!;
    private IUserDbContextFactory _factory = null!;

    public async ValueTask InitializeAsync()
    {
        _fixture = new PostgresFixture();
        await _fixture.InitializeAsync();
        _factory = new TestDbContextFactory(_fixture.DbOptions);


        _roleService = new RoleService(_factory);
    }

    public async ValueTask DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task GetRoleItems_ValidRoleItems_ShouldReturnRoleItems()
    {
        var results = await _roleService.GetList();

        results.Value.roleItems.Should().NotBeNullOrEmpty();
    }

}