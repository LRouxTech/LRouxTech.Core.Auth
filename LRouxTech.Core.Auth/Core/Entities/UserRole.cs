using LRouxTech.Core.BaseModel;

namespace LRouxTech.Core.Auth.Core.Entities;

public class UserRole : BaseModel<UserRole>
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public Guid RoleId { get; set; }
    public virtual Role Role { get; set; }
}