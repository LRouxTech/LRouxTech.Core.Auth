using System.Collections;
using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using LRouxTech.Core.Auth.Infrastructure.Errors;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LRouxTech.Core.AuthTests.TestData.Arguments;

public class PasswordCreationValidationArgs : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            UserData.ExistingGuid,
            PasswordCreation() with { token = "" },
            TokenErrors.EmptyToken,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            PasswordCreation() with { password = "" },
            PasswordErrors.EmptyPassword,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            PasswordCreation() with { passwordConfirmation = "" },
            PasswordErrors.EmptyConfirmPassword,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            PasswordCreation() with { password = "password789#" },
            PasswordErrors.PasswordsdontMatch,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            PasswordCreation() with { passwordConfirmation = "password789#" },
            PasswordErrors.PasswordsdontMatch,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            PasswordCreation() with { password = "p789#", passwordConfirmation = "p789#"},
            PasswordErrors.TooShort,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            PasswordCreation() with { password = "password789#", passwordConfirmation = "password789#"},
            PasswordErrors.NoUppercase,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            PasswordCreation() with { password = "PASSWORD789#", passwordConfirmation = "PASSWORD789#"},
            PasswordErrors.NoLowercase,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            PasswordCreation() with { password = "PASSWORddd", passwordConfirmation = "PASSWORddd"},
            PasswordErrors.NoNumber,
            typeof(BadRequest)
        };
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public PasswordCreationRequest PasswordCreation()
    {
        return new PasswordCreationRequest
        (
            "token",
            "Password123#",
            "Password123#"
        );
    }
}