using FluentAssertions;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.Auth.Infrastructure.Services;
using LRouxTech.Core.AuthTests.TestData.Setup;

namespace LRouxTech.Core.AuthTests.UnitTests.ServiceTests;

public class PermissionServiceTests : IAsyncLifetime
{
    private PermissionService _PermissionService;
    private PostgresFixture _fixture = null!;
    private IUserDbContextFactory _factory = null!;

    public async ValueTask InitializeAsync()
    {
        _fixture = new PostgresFixture();
        await _fixture.InitializeAsync();
        _factory = new TestDbContextFactory(_fixture.DbOptions);


        _PermissionService = new PermissionService(_factory);
    }

    public async ValueTask DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task GetPermissionItems_ValidPermissionItems_ShouldReturnPermissionItems()
    {
        var results = await _PermissionService.GetList();

        results.Value.permissionItems.Should().NotBeNullOrEmpty();
    }

}