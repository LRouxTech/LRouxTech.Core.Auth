using LRouxTech.Core.BaseModel;

namespace LRouxTech.Core.Auth.Core.Entities;

public class RolePermission : BaseModel<RolePermission>
{
    public Guid RoleId { get; set; }
    public virtual Role Role { get; set; }
    public Guid PermissionId { get; set; }
    public virtual Permission Permission { get; set; }
}