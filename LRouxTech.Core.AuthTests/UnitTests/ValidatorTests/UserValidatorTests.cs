using FluentAssertions;
using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using LRouxTech.Core.Auth.Infrastructure.Validator;
using LRouxTech.Core.AuthTests.TestData.Arguments;
using LRouxTech.Core.AuthTests.TestData.EntityData;
using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.AuthTests.UnitTests.ValidatorTests;

public class UserValidatorTests
{
    private IUserValidator? _userValidator;

    private void SetUp()
    {
        _userValidator = new UserValidator();
    }
    
    [Theory]
    [InlineData("NewName", "NewSurname", "NewUsername", "NewEmail", true)]
    public void ValidateUserCreation_ValidUser_Valid(string name, string surname, string username, string email, bool isValid)
    {
        // Arrange
        var request = new CreateUserRequest
        (
            name,
            surname,
            username,
            email,
            [RoleData.ExistingGuid],
            [PermissionData.ExistingGuid]
        );
        SetUp();
        
        // Act
        var actualResult = _userValidator!.ValidateUserCreation(request);
    
        // Assert
        actualResult.IsSuccess.Should().BeTrue(); 
        actualResult.Value.Should().Be(true);
    }
    
    [Theory]
    [ClassData(typeof(UserCreationValidationArgs))]
    public void ValidateUserCreation_InvalidUser_ReturnsError(Guid userId, CreateUserRequest request, Result<bool> result, Type exceptionType)
    {
        // Arrange
        SetUp();
        
        // Act
        var actualResult = _userValidator!.ValidateUserCreation(request);
    
        // Assert
        actualResult.IsFailure.Should().BeTrue(); 
        actualResult.Error.Should().Be(result.Error);
    }
    
    [Theory]
    [InlineData("NewName", "NewSurname", "NewUsername", "NewEmail", true)]
    public void ValidateUserUpdate_ValidUser_Valid(string name, string surname, string username, string email, bool isValid)
    {
        // Arrange
        var request = new UpdateUserRequest
        (
            UserData.ExistingGuid,
            name,
            surname,
            username,
            email,
            [RoleData.ExistingGuid],
            [PermissionData.ExistingGuid]
        );
        SetUp();
        
        // Act
        var actualResult = _userValidator!.ValidateUserUpdate(request);
    
        // Assert
        actualResult.IsSuccess.Should().BeTrue(); 
        actualResult.Value.Should().Be(true);
    }
    
    [Theory]
    [ClassData(typeof(UserUpdateValidationArgs))]
    public void ValidateUserUpdate_InvalidUser_ReturnsError(Guid userId, UpdateUserRequest request, Result<bool> result, Type exceptionType)
    {
        // Arrange
        SetUp();
        
        // Act
        var actualResult = _userValidator!.ValidateUserUpdate(request);
    
        // Assert
        actualResult.IsFailure.Should().BeTrue(); 
        actualResult.Error.Should().Be(result.Error);
    }
    
    [Theory]
    [InlineData("token", "Password123", "Password123")]
    public void ValidatePasswordCreation_ValidPassword_Valid(string token, string password, string passwordConfirm)
    {
        // Arrange
        var request = new PasswordCreationRequest
        (
            token,
            password,
            passwordConfirm
        );
        SetUp();
        
        // Act
        var actualResult = _userValidator!.ValidateUserPasswordCreation(request);
    
        // Assert
        actualResult.IsSuccess.Should().BeTrue(); 
        actualResult.Value.Should().Be(true);
    }
    
    [Theory]
    [ClassData(typeof(PasswordCreationValidationArgs))]
    public void ValidatePasswordCreation_InvalidPassword_ReturnsError(Guid userId, PasswordCreationRequest request, Result<bool> result, Type exceptionType)
    {
        // Arrange
        SetUp();
        
        // Act
        var actualResult = _userValidator!.ValidateUserPasswordCreation(request);
    
        // Assert
        actualResult.IsFailure.Should().BeTrue(); 
        actualResult.Error.Should().Be(result.Error);
    }
    
    [Theory]
    [InlineData("token", "Password123", "Password123")]
    public void ValidatePasswordUpdate_ValidPassword_Valid(string token, string password, string passwordConfirm)
    {
        // Arrange
        var request = new UpdatePasswordRequest
        (
            token,
            password,
            passwordConfirm
        );
        SetUp();
        
        // Act
        var actualResult = _userValidator!.ValidateUserPasswordUpdate(request);
    
        // Assert
        actualResult.IsSuccess.Should().BeTrue(); 
        actualResult.Value.Should().Be(true);
    }
    
    [Theory]
    [ClassData(typeof(PasswordUpdateValidationArgs))]
    public void ValidatePasswordUpdate_InvalidPassword_ReturnsError(Guid userId, UpdatePasswordRequest request, Result<bool> result, Type exceptionType)
    {
        // Arrange
        SetUp();
        
        // Act
        var actualResult = _userValidator!.ValidateUserPasswordUpdate(request);
    
        // Assert
        actualResult.IsFailure.Should().BeTrue(); 
        actualResult.Error.Should().Be(result.Error);
    }
}