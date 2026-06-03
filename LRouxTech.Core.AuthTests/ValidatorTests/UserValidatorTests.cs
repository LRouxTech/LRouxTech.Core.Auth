using FluentAssertions;
using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using LRouxTech.Core.Auth.Infrastructure.Validator;
using LRouxTech.Core.AuthTests.TestData.Arguments;
using LRouxTech.Core.ValidationResult;

namespace LRouxTech.Core.AuthTests.ValidatorTests;

public class UserValidatorTests
{
    private IUserValidator? _userValidator;

    private void SetUp()
    {
        _userValidator = new UserValidator();
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