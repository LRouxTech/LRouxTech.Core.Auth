public class UserPermission : BaseModel<UserPermission>
{
    public Guid UserId { get; set; }
    public Guid PermissionId { get; set; }
}
