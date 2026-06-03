using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.BaseModel;

namespace LRouxTech.Core.AuthTests.TestData.EntityData;

public static class PermissionData
{
    public static readonly Guid ExistingGuid = Guid.NewGuid();

    public static async Task SeedData(UserContext dbContext)
    {
        var data = new List<Permission>
        {
            CreateExistingSave(),
            new Permission()
            {
                Section = "Section2",
                PermissionName = "Permission2",
                Description = "Description2",
            }.Create(),
            new Permission()
            {
                Section = "Section3",
                PermissionName = "Permission3",
                Description = "Description3",
            }.Create(),
            new Permission()
            {
                Section = "Section4",
                PermissionName = "Permission4",
                Description = "Description4",
            }.Create(),
        };
        
        dbContext.AddRange(data);
        await dbContext.SaveChangesAsync();
    }
    
    public static Permission CreateExistingSave()
    {
        return new Permission()
        {
            Id = ExistingGuid,
            Section = "Section1",
            PermissionName = "Permission1",
            Description = "Description1",
            CreatedById = BaseModelConstant.SystemId,
            CreatedOn = DateTime.Now,
            UpdatedById = BaseModelConstant.SystemId,
            UpdatedOn = DateTime.Now
        };
    }

}