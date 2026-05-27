public class Permission : BaseModel<Permission>
{
    public string Section { get; set; }
    public string PermissionName { get; set; }
    public string Description { get; set; }
}
