using LRouxTech.Core.BaseModel;

namespace LRouxTech.Core.Auth.Core.Entities;

public class User : BaseModel<User>
{
    public required string Name { get; set; }
    public string? Surname { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public byte[] PasswordHash { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
    public ICollection<UserPermission> UserPermissions { get; set; }
    public ICollection<UserToken> UserTokens { get; set; }
}