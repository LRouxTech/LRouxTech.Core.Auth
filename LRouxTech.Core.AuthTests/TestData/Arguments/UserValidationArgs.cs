using System.Collections;
using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using LRouxTech.Core.Auth.Infrastructure.Errors;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LRouxTech.Core.AuthTests.TestData.Arguments;

public class UserCreateValidationArgs : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            UserData.ExistingGuid,
            CreateNewUser() with { Username = "" },
            UserErrors.NoUsername,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            CreateNewUser() with { Email = "" },
            UserErrors.NoEmail,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            CreateNewUser() with { RoleIds = [] },
            RoleErrors.NoRole,
            typeof(BadRequest)
        };
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public CreateUserRequest CreateNewUser()
    {
        return new CreateUserRequest
        (
            "NewName",
            "NewSurname",
            "NewUsername",
            "NewEmail",
            [RoleData.ExistingGuid],
            [PermissionData.ExistingGuid]
        );
    }
}