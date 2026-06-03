using FluentAssertions;
using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.Auth.Infrastructure.Errors;
using LRouxTech.Core.Auth.Infrastructure.Services;
using LRouxTech.Core.AuthTests.Helpers;
using LRouxTech.Core.AuthTests.TestData.EntityData;
using LRouxTech.Core.AuthTests.TestData.Setup;
using Moq;

namespace LRouxTech.Core.AuthTests.UnitTests.ServiceTests;

public class UserServiceTests : IAsyncLifetime
{
    private UserService _userService;
    private PostgresFixture _fixture = null!;
    private IUserDbContextFactory _factory = null!;

    public async ValueTask InitializeAsync()
    {
        _fixture = new PostgresFixture();
        await _fixture.InitializeAsync();
        _factory = new TestDbContextFactory(_fixture.DbOptions);


        _userService = (UserService)UserServiceHelper.CreateUserService(_factory);
    }

    public async ValueTask DisposeAsync()
    {
        await _fixture.DisposeAsync();
    }

    [Fact]
    public async Task UserLogin_ValidUser_ShouldReturnSignedInUser()
    {
        var result = await _userService.Login(new UserLoginRequest(UserData.UserName, UserData.Password));
        
        result.IsSuccess.Should().BeTrue();
        result.Value.UserName.Should().Be(UserData.UserName);
        result.Value.tokenValue.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public async Task UserLogin_InvalidUser_ShouldReturnError()
    {
        var result = await _userService.Login(new UserLoginRequest("FakeUserName", UserData.Password));
        
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.UserNotFound);
    }
    
    [Fact]
    public async Task UserLogin_InvalidPassword_ShouldReturnError()
    {
        var result = await _userService.Login(new UserLoginRequest(UserData.UserName, "FakePassword"));
        
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidPassword);
    }
    
    [Fact]
    public async Task UserLogout_ValidUser_ShouldReturnTrue()
    {
        var result = await _userService.Logout(new UserLogoutRequest(UserData.ExistingGuid));
        
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(true);
    }
    
    [Fact]
    public async Task UserLogout_InvalidUser_ShouldReturnError()
    {
        var result = await _userService.Logout(new UserLogoutRequest(Guid.CreateVersion7()));
        
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.UserNotFound);
    }
    
    [Fact]
    public async Task UserCreateValidUser_ShouldReturnUser()
    {
        var name = "NewName";
        var surname = "NewSurname";
        var username = "NewUsername";
        var email = "NewEmail";
        var newUser =  new CreateUserRequest
        (
            name,
            surname,
            username,
            email,
            [RoleData.ExistingGuid],
            [PermissionData.ExistingGuid]
        );
        var result = await _userService.Create(newUser);
        
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(name);
        result.Value.Surname.Should().Be(surname);
        result.Value.UserName.Should().Be(username);
        result.Value.Email.Should().Be(email);
        
        result.Value.UserRoles.Should().NotBeNullOrEmpty();
        result.Value.UserPermissions.Should().NotBeNullOrEmpty();
    }

}