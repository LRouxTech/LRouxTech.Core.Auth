using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.BaseModel;

namespace LRouxTech.Core.AuthTests.TestData.EntityData;

public class RoleData
{
    public static readonly Guid ExistingGuid = Guid.NewGuid();

    public static async Task SeedData(UserContext dbContext)
    {
        var data = new List<Role>
        {
            CreateExistingSave(),
            new Role()
            {
                Name = "Name2",
                Description = "Description2",
            }.Create(),
            new Role()
            {
                Name = "Name3",
                Description = "Description3",
            }.Create(),
            new Role()
            {
                Name = "Name4",
                Description = "Description4",
            }.Create(),
        };
        
        dbContext.AddRange(data);
        await dbContext.SaveChangesAsync();
    }
    
    public static Role CreateExistingSave()
    {
        return new Role()
        {
            Id = ExistingGuid,
            Name = "Name1",
            Description = "Description1",
            CreatedById = BaseModelConstant.SystemId,
            CreatedOn = DateTime.UtcNow,
            UpdatedById = BaseModelConstant.SystemId,
            UpdatedOn = DateTime.UtcNow
        };
    }
}