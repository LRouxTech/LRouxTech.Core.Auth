using LRouxTech.Core.BaseModel;

namespace LRouxTech.Core.Auth.Core.Entities;

public class Permission : BaseModel<Permission>
{
    public string Section { get; set; }
    public string PermissionName { get; set; }
    public string Description { get; set; }
    public virtual ICollection<RolePermission> RolePermissions { get; set; }
    public virtual ICollection<UserPermission> UserPermissions { get; set; }
}