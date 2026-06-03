using System.Collections;
using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using LRouxTech.Core.Auth.Infrastructure.Errors;
using LRouxTech.Tests.Auth.TestData.EntityData;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LRouxTech.Tests.Auth.TestData.Arguments;

public class PasswordUpdateValidationArgs : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            UserData.ExistingGuid,
            PasswordUpdate() with { token = "" },
            TokenErrors.EmptyToken,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            PasswordUpdate() with { newPassword = "" },
            PasswordErrors.EmptyPassword,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            PasswordUpdate() with { newPasswordConfirm = "" },
            PasswordErrors.EmptyConfirmPassword,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            PasswordUpdate() with { newPassword = "password789#" },
            PasswordErrors.PasswordsdontMatch,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            PasswordUpdate() with { newPasswordConfirm = "password789#" },
            PasswordErrors.PasswordsdontMatch,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            PasswordUpdate() with { newPassword = "p789#", newPasswordConfirm = "p789#"},
            PasswordErrors.TooShort,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            PasswordUpdate() with { newPassword = "password789#", newPasswordConfirm = "password789#"},
            PasswordErrors.NoUppercase,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            PasswordUpdate() with { newPassword = "PASSWORD789#", newPasswordConfirm = "PASSWORD789#"},
            PasswordErrors.NoLowercase,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            PasswordUpdate() with { newPassword = "PASSWORddd", newPasswordConfirm = "PASSWORddd"},
            PasswordErrors.NoNumber,
            typeof(BadRequest)
        };
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public UpdatePasswordRequest PasswordUpdate()
    {
        return new UpdatePasswordRequest
        (
            "token",
            "Password123#",
            "Password123#"
        );
    }
}