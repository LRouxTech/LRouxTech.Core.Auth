using LRouxTech.Core.BaseModel;

namespace LRouxTech.Core.Auth.Core.Entities;

public class UserPermission : BaseModel<UserPermission>
{
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public Guid PermissionId { get; set; }
    public virtual Permission Permission { get; set; }
}