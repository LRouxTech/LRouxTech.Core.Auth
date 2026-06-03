using FluentAssertions;
using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.Auth.Infrastructure.Errors;
using LRouxTech.Core.Auth.Infrastructure.Services;
using LRouxTech.Core.AuthTests.TestData.EntityData;
using LRouxTech.Core.AuthTests.TestData.Setup;
using Microsoft.Extensions.Configuration;
using Moq;

namespace LRouxTech.Core.AuthTests.ServiceTests;

public class TokenServiceTests : IAsyncLifetime
{
    private TokenService _tokenService;
    private PostgresFixture _fixture = null!;
    private IUserDbContextFactory _factory = null!;
    private Mock<IConfiguration> _mockConfiguration = null!;

    public async ValueTask InitializeAsync()
    {
        _fixture = new PostgresFixture();
        await _fixture.InitializeAsync();
        _factory = new TestDbContextFactory(_fixture.DbOptions);

        _mockConfiguration  =new Mock<IConfiguration>(); 
        _mockConfiguration
            .Setup(c => c["JwtSettings:SecretKey"])
            .Returns("super-secret-test-key-that-is-long-enough");

        _mockConfiguration
            .Setup(c => c["JwtSettings:Issuer"])
            .Returns("https://test-issuer.com");

        _mockConfiguration
            .Setup(c => c["JwtSettings:Audience"])
            .Returns("https://test-audience.com");

        _tokenService = new TokenService(_factory, _mockConfiguration.Object);
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
    public async Task GenerateUserToken_InvalidUser_ShouldReturnValidToken()
    {
        await using var dbContext = await _factory.CreateDbContextAsync(TestContext.Current.CancellationToken);
        var results = await _tokenService.GenerateToken(UserData.ExistingGuid);
        
        results.IsFailure.Should().BeTrue();
        results.Error.Should().NotBeNull();
        results.Error.Should().Be(UserErrors.UserNotFound);
    }
    
}