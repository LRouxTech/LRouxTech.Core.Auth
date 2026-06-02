using LRouxTech.Core.Auth.Core.Entities;

namespace LRouxTech.Core.AuthTests.TestData;

public static class PermissionData
{
    public static readonly Guid ExistingGuid = Guid.NewGuid();
    
    public static Permission CreateExistingSave()
    {
        return new Permission()
        {
            Id = ExistingGuid,
            Section = "Section1",
            Description = "Description1",
            
            CreatedById = UserData.System,
            UpdatedById = UserData.System,
        };
    }

}