public class RolePermission : BaseModel<RolePermission>
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
}
