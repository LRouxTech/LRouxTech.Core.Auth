using LRouxTech.Core.Auth.Core.Interfaces;
using LRouxTech.Core.Auth.Infrastructure.Validator;

namespace LRouxTech.Core.AuthTests.ValidatorTests;

public class UserValidatorTests
{
    private IUserValidator? _userValidator;

    private void SetUp()
    {
        _userValidator = new UserValidator();
    }
    
    [Theory]
    [ClassData(typeof())]
    public void ValidateUser_ValidUser_Success()
    {
        
    }
}