using FluentAssertions;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.Auth.Infrastructure.Errors;
using LRouxTech.Core.Auth.Infrastructure.Services;
using LRouxTech.Tests.Auth.Helpers;
using LRouxTech.Tests.Auth.TestData.EntityData;
using LRouxTech.Tests.Auth.TestData.Setup;
using Microsoft.Extensions.Configuration;
using Moq;

namespace LRouxTech.Tests.Auth.UnitTests.ServiceTests;

public class TokenServiceTests : IAsyncLifetime
{
    private TokenService _tokenService;
    private PostgresFixture _fixture = null!;
    private IUserDbContextFactory _factory = null!;

    public async ValueTask InitializeAsync()
    {
        _fixture = new PostgresFixture();
        await _fixture.InitializeAsync();
        _factory = new TestDbContextFactory(_fixture.DbOptions);
        

        _tokenService = new TokenService(_factory, ConfigurationMockHelper.CreateConfigurationMock().Object);
    }
    
    public async ValueTask DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }
    
    [Fact]
    public async Task GenerateUserToken_ValidUser_ShouldReturnValidToken()
    {
        await using var dbContext = await _factory.CreateDbContextAsync(TestContext.Current.CancellationToken);
        var results = await _tokenService.GenerateToken(UserData.ExistingGuid);
        
       results.IsSuccess.Should().BeTrue();
       results.Value.Should().NotBeNull();
       results.Value.TokenValue.Should().NotBeNullOrEmpty();
       results.Value.Expired.Should().BeFalse();
    }
    
    [Fact]
    public async Task GenerateUserToken_InvalidUser_ShouldReturnError()
    {
        await using var dbContext = await _factory.CreateDbContextAsync(TestContext.Current.CancellationToken);
        var results = await _tokenService.GenerateToken(Guid.CreateVersion7());
        
        results.IsFailure.Should().BeTrue();
        results.Error.Should().NotBeNull();
        results.Error.Should().Be(UserErrors.UserNotFound);
    }
    
    [Fact]
    public async Task GetUserToken_InvalidUser_ShouldReturnError()
    {
        await using var dbContext = await _factory.CreateDbContextAsync(TestContext.Current.CancellationToken);
        var results = await _tokenService.GetToken(Guid.CreateVersion7());
        
        results.IsFailure.Should().BeTrue();
        results.Error.Should().NotBeNull();
        results.Error.Should().Be(UserErrors.UserNotFound);
    }
    
    [Fact]
    public async Task GetNewUserToken_ValidUser_ShouldReturnValidToken()
    {
        await using var dbContext = await _factory.CreateDbContextAsync(TestContext.Current.CancellationToken);
        var results = await _tokenService.GetToken(UserData.ExistingGuid);
        
        results.IsSuccess.Should().BeTrue();
        results.Value.Should().NotBeNull();
        results.Value.TokenValue.Should().NotBeNullOrEmpty();
        results.Value.Expired.Should().BeFalse();
    }
    
    [Fact]
    public async Task GetExistingUserToken_ValidUser_ShouldReturnValidToken()
    { 
        await using var dbContext = await _factory.CreateDbContextAsync(TestContext.Current.CancellationToken);
        await _tokenService.GenerateToken(UserData.ExistingGuid);
        
        var results = await _tokenService.GetToken(UserData.ExistingGuid);
        
        results.IsSuccess.Should().BeTrue();
        results.Value.Should().NotBeNull();
        results.Value.TokenValue.Should().NotBeNullOrEmpty();
        results.Value.Expired.Should().BeFalse();
    }
    
    [Fact]
    public async Task ValidateUserToken_ValidToken_ShouldReturnValidToken()
    { 
        await using var dbContext = await _factory.CreateDbContextAsync(TestContext.Current.CancellationToken);
        var newToken = await _tokenService.GenerateToken(UserData.ExistingGuid);
        
        var results = await _tokenService.ValidateToken(newToken.Value.TokenValue);
        
        results.IsSuccess.Should().BeTrue();
        results.Value.Should().NotBeNull();
        results.Value.TokenValue.Should().NotBeNullOrEmpty();
        results.Value.Expired.Should().BeFalse();
    }
    
    [Fact]
    public async Task ValidateUserToken_InvalidToken_ShouldReturnError()
    { 
        await using var dbContext = await _factory.CreateDbContextAsync(TestContext.Current.CancellationToken);
        var results = await _tokenService.ValidateToken("FakeToken");
        
        results.IsFailure.Should().BeTrue();
        results.Error.Should().NotBeNull();
        results.Error.Should().Be(TokenErrors.TokenNotFound);
    }
    
    [Fact]
    public async Task InvalidateUserToken_ValidToken_ShouldReturnTrue()
    { 
        await using var dbContext = await _factory.CreateDbContextAsync(TestContext.Current.CancellationToken);
        var newToken = await _tokenService.GenerateToken(UserData.ExistingGuid);
        
        var results = await _tokenService.InvalidateToken(newToken.Value.TokenValue);
        
        results.IsSuccess.Should().BeTrue();
        results.Value.Should().Be(true);
    }
    
    [Fact]
    public async Task InvalidateUserToken_InvalidToken_ShouldReturnTrue()
    { 
        await using var dbContext = await _factory.CreateDbContextAsync(TestContext.Current.CancellationToken);
        
        var results = await _tokenService.InvalidateToken("FakeToken");
        
        results.IsSuccess.Should().BeTrue();
        results.Value.Should().Be(true);
    }
    
    [Fact]
    public async Task InvalidateAllUserTokens_ValidToken_ShouldReturnTrue()
    { 
        await using var dbContext = await _factory.CreateDbContextAsync(TestContext.Current.CancellationToken);
        var newToken = await _tokenService.GenerateToken(UserData.ExistingGuid);

        var results = await _tokenService.InvalidateToken(newToken.Value.TokenValue);

        results.IsSuccess.Should().BeTrue();
        results.Value.Should().Be(true);
    }
    
    [Fact]
    public async Task InvalidateAllUserTokens_InvalidToken_ShouldReturnTrue()
    { 
        await using var dbContext = await _factory.CreateDbContextAsync(TestContext.Current.CancellationToken);
        
        var results = await _tokenService.InvalidateToken("FakeToken");
        
        results.IsSuccess.Should().BeTrue();
        results.Value.Should().Be(true);
    }
    
}