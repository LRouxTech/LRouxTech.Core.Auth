using LRouxTech.Core.Auth.Core.Entities;
using LRouxTech.Core.Auth.Infrastructure.Database;
using LRouxTech.Core.BaseModel;

namespace LRouxTech.Core.AuthTests.TestData;

public static class UserData
{
    public static readonly Guid ExistingGuid = Guid.NewGuid();

    public static async Task SeedData(UserContext dbContext)
    {
        var data = new List<User>
        {
            CreateExistingSave(),
            new User()
            {
                Name = "Name2",
                Surname = "Surname2",
                Email = "Email@gmail.com2",
                PasswordHash = new byte[32],
                UserName = "UserName2",
            }.Create(),
            new User()
            {
                Name = "Name3",
                Surname = "Surname3",
                Email = "Email@gmail.com3",
                PasswordHash = new byte[32],
                UserName = "UserName3",
            }.Create(),
            new User()
            {
                Name = "Name4",
                Surname = "Surname4",
                Email = "Email@gmail.com4",
                PasswordHash = new byte[32],
                UserName = "UserName4",
            }.Create(),
        };
    }
    
    public static User CreateExistingSave()
    {
        return new User()
        {
            Id = ExistingGuid,
            Name = "Name1",
            Surname = "Surname1",
            Email = "Email@gmail.com1",
            PasswordHash = new byte[32],
            UserName = "UserName1",
            CreatedById = BaseModelConstant.SystemId,
            CreatedOn = DateTime.Now,
            UpdatedById = BaseModelConstant.SystemId,
            UpdatedOn = DateTime.Now
        };
    }

}