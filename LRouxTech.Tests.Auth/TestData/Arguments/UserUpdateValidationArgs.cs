using System.Collections;
using LRouxTech.Core.Auth.Core.ViewModels.User.Request;
using LRouxTech.Core.Auth.Infrastructure.Errors;
using LRouxTech.Tests.Auth.TestData.EntityData;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LRouxTech.Tests.Auth.TestData.Arguments;

public class UserUpdateValidationArgs : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            UserData.ExistingGuid,
            UpdateUser() with { UserId = Guid.Empty },
            UserErrors.NoUserId,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            UpdateUser() with { Username = "" },
            UserErrors.NoUsername,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            UpdateUser() with { Email = "" },
            UserErrors.NoEmail,
            typeof(BadRequest)
        };
        
        yield return new object[]
        {
            UserData.ExistingGuid,
            UpdateUser() with { RoleIds = [] },
            RoleErrors.NoRole,
            typeof(BadRequest)
        };
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public UpdateUserRequest UpdateUser()
    {
        return new UpdateUserRequest
        (
            UserData.ExistingGuid,
            "NewUsername",
            "NewName",
            "NewSurname",
            "NewEmail",
            [RoleData.ExistingGuid],
            [PermissionData.ExistingGuid]
        );
    }
}