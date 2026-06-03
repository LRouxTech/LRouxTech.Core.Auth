using FluentAssertions;
using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.Auth.Infrastructure.Errors;
using LRouxTech.Core.Auth.Infrastructure.Services;
using LRouxTech.Core.AuthTests.Helpers;
using LRouxTech.Core.AuthTests.TestData.Arguments;
using LRouxTech.Core.AuthTests.TestData.EntityData;
using LRouxTech.Core.AuthTests.TestData.Setup;
using LRouxTech.Core.ValidationResult;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace LRouxTech.Core.AuthTests.UnitTests.ServiceTests;

public class UserServiceTests : IAsyncLifetime
{
    private UserService _userService;
    private TokenService _tokenService;
    private PostgresFixture _fixture = null!;
    private IUserDbContextFactory _factory = null!;

    public async ValueTask InitializeAsync()
    {
        _fixture = new PostgresFixture();
        await _fixture.InitializeAsync();
        _factory = new TestDbContextFactory(_fixture.DbOptions);


        _userService = (UserService)UserServiceHelper.CreateUserService(_factory);
        _tokenService = new TokenService(_factory, ConfigurationMockHelper.CreateConfigurationMock().Object);
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
    public async Task CreateUser_ValidUser_ShouldReturnUser()
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
    
    [Theory]
    [ClassData(typeof(UserCreationValidationArgs))]
    public async Task CreateUser_InvalidUser_ShouldReturnUser(Guid userId, CreateUserRequest request, Result<bool> result, Type exceptionType)
    {
        var actualResult = await _userService.Create(request);
        
        actualResult.IsFailure.Should().BeTrue(); 
        actualResult.Error.Should().Be(result.Error);
    }
    
    [Fact]
    public async Task UpdateUser_ValidUser_ShouldReturnUser()
    {
        var name = "NewName";
        var surname = "NewSurname";
        var username = "NewUsername";
        var email = "NewEmail";
        var newUser = new UpdateUserRequest
        (
            UserData.ExistingGuid,
            username,
            name,
            surname,
            email,
            [RoleData.ExistingGuid],
            [PermissionData.ExistingGuid]
        );
        var result = await _userService.Update(newUser);
        
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(name);
        result.Value.Surname.Should().Be(surname);
        result.Value.UserName.Should().Be(username);
        result.Value.Email.Should().Be(email);
        
        result.Value.UserRoles.Should().NotBeNullOrEmpty();
        result.Value.UserPermissions.Should().NotBeNullOrEmpty();
    }
    
    [Theory]
    [ClassData(typeof(UserUpdateValidationArgs))]
    public async Task UserCreateUser_InvalidUser_ShouldReturnUser(Guid userId, UpdateUserRequest request, Result<bool> result, Type exceptionType)
    {
        var actualResult = await _userService.Update(request);
        
        actualResult.IsFailure.Should().BeTrue(); 
        actualResult.Error.Should().Be(result.Error);
    }
    
    [Fact]
    public async Task AuthenitcateUser_ValidToken_ShouldReturnUser()
    {
        var dbContext = await _factory.CreateDbContextAsync(TestContext.Current.CancellationToken);
        var token = await _tokenService.GenerateToken(UserData.ExistingGuid);
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == UserData.ExistingGuid, cancellationToken: TestContext.Current.CancellationToken);
        var result = await _userService.Authenticate(new AuthenticateUserRequest(token.Value.TokenValue));
        
        result.IsSuccess.Should().BeTrue();
        result.Value.UserName.Should().Be(user.UserName);
        result.Value.Email.Should().Be(user.Email);
        result.Value.tokenValue.Should().Be(token.Value.TokenValue);
    }
    
    [Fact]
    public async Task AuthenitcateUser_InvalidToken_ShouldReturnUser()
    {
        var result = await _userService.Authenticate(new AuthenticateUserRequest("token"));
        
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TokenErrors.TokenNotFound);
    }
    
    [Fact]
    public async Task GetUsers_ActiveUsers_ShouldReturnUsers()
    {
        var results = await _userService.GetUserList(new UserListRequest(true));

        results.Value.Users.Should().NotBeNullOrEmpty();
    }
    
    [Fact]
    public async Task GetUser_ValidUser_ShouldReturnUser()
    {
        var dbContext = await _factory.CreateDbContextAsync(TestContext.Current.CancellationToken);
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == UserData.ExistingGuid, cancellationToken: TestContext.Current.CancellationToken);
        var result = await _userService.GetUser(new UserDetailRequest(UserData.ExistingGuid));

        result.IsSuccess.Should().Be(true);
        result.Value.UserName.Should().Be(user.UserName);
        result.Value.Email.Should().Be(user.Email);
    }
    
    [Fact]
    public async Task GetUser_InvalidUser_ShouldReturnError()
    {
        var result = await _userService.GetUser(new UserDetailRequest(Guid.CreateVersion7()));

        result.IsSuccess.Should().Be(true);
        result.Error.Should().Be(UserErrors.UserNotFound);
    }

    [Fact]
    public async Task InitialPasswordSet_ValidPassword_ShouldReturnTrue()
    {
        var token = await _tokenService.GenerateToken(UserData.ExistingGuid);
        var result = await _userService.InitialPasswordSet(new PasswordCreationRequest(token.Value.TokenValue, UserData.Password, UserData.Password ));

        result.IsSuccess.Should().Be(true);
        result.Value.Should().Be(true);
    }
    
    [Theory]
    [ClassData(typeof(PasswordCreationValidationArgs))]
    public async Task InitialPasswordSet_Invalid_ShouldReturnError(Guid userId, PasswordCreationRequest request, Result<bool> result, Type exceptionType)
    {
        if (request.token != "")
        {
            var token = await _tokenService.GenerateToken(UserData.ExistingGuid);
            request = request with { token = token.Value.TokenValue };
        }
        var actualResult = await _userService.InitialPasswordSet(request);

        actualResult.IsFailure.Should().Be(true);
        actualResult.Error.Should().Be(result.Error);
    }
    
    [Fact]
    public async Task UpdatePassword_ValidPassword_ShouldReturnTrue()
    {
        var token = await _tokenService.GenerateToken(UserData.ExistingGuid);
        var result = await _userService.UpdatePassword(new UpdatePasswordRequest(token.Value.TokenValue, UserData.Password, UserData.Password ));

        result.IsSuccess.Should().Be(true);
        result.Value.Should().Be(true);
    }
    
    [Theory]
    [ClassData(typeof(PasswordUpdateValidationArgs))]
    public async Task UpdatePassword_Invalid_ShouldReturnError(Guid userId, UpdatePasswordRequest request, Result<bool> result, Type exceptionType)
    {
        if (request.token != "")
        {
            var token = await _tokenService.GenerateToken(UserData.ExistingGuid);
            request = request with { token = token.Value.TokenValue };
        }
        var actualResult = await _userService.UpdatePassword(request);

        actualResult.IsFailure.Should().Be(true);
        actualResult.Error.Should().Be(result.Error);
    }
    
    [Fact]
    public async Task ResetPassword_ValidEmail_ShouldReturnTrue()
    {
        var result = await _userService.ResetPassword(new ResetPasswordRequest(UserData.Email));

        result.IsSuccess.Should().Be(true);
        result.Value.Should().Be(true);
    }
    
    [Fact]
    public async Task ResetPassword_InvalidEmail_ShouldReturnFalse()
    {
        var result = await _userService.ResetPassword(new ResetPasswordRequest("InvalidEmail@gmail.com"));

        result.IsFailure.Should().Be(true);
        result.Error.Should().Be(UserErrors.UserNotFound);
    }
    
    [Fact]
    public async Task ArchiveUser_ValidUser_ShouldReturnTrue()
    {
        var result = await _userService.ArchiveUser(new ArchiveUserRequest(UserData.ExistingGuid),  UserData.ExistingGuid);

        result.IsSuccess.Should().Be(true);
        result.Value.Should().Be(true);
    }
    
    [Fact]
    public async Task ArchiveUser_InvalidUser_ShouldReturnFalse()
    {
        var result = await _userService.ArchiveUser(new ArchiveUserRequest(Guid.CreateVersion7()),  UserData.ExistingGuid);

        result.IsFailure.Should().Be(true);
        result.Error.Should().Be(UserErrors.UserNotFound);
    }
    
    [Fact]
    public async Task DeleteUser_ValidUser_ShouldReturnTrue()
    {
        var dbContext = await _factory.CreateDbContextAsync(TestContext.Current.CancellationToken);
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == UserData.ExistingGuid, cancellationToken: TestContext.Current.CancellationToken);
        user.ArchivedOn = DateTime.UtcNow;
        dbContext.Update(user);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        
        var result = await _userService.DeleteUser(new DeleteUserRequest(UserData.ExistingGuid));

        result.IsSuccess.Should().Be(true);
        result.Value.Should().Be(true);
    }

    [Fact] public async Task DeleteUser_InvalidUser_ShouldReturnFalse()
    {
        var result = await _userService.DeleteUser(new DeleteUserRequest(Guid.CreateVersion7()));

        result.IsFailure.Should().Be(true);
        result.Error.Should().Be(UserErrors.UserNotFound);
    }
}