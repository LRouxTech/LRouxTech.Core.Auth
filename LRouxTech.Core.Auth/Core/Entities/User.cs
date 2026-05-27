namespace LRouxTech.Core.Auth.Models;

public class User : BaseModel<User>
{
    public required string Name { get; set; }
    public string? Surname { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
}
