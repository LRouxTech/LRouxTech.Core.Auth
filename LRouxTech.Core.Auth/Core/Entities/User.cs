using LRouxTech.Core.BaseModel;

namespace LRouxTech.Core.Auth.Core.Entities;

public class User : BaseModel<User>
{
    public required string Name { get; set; }
    public string? Surname { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public string PasswordHash { get; set; }
}