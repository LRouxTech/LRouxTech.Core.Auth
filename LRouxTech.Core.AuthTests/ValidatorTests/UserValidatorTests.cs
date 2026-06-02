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
    [ClassData(typeof(UserCreateValidationArgs))]
    public void ValidateUser_ValidUser_Success(Guid userId, CreateUserRequest request, Result<bool> result, Type exceptionType)
    {
        // Arrange
        SetUp();
        
        // Act
        var actualResult = _userValidator!.ValidateUserCreation(request);
    
        // Assert
        actualResult.IsFailure.Should().BeTrue(); 
        actualResult.Error.Should().Be(result.Error);
    }
}