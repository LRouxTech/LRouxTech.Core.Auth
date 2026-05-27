public class UserRole : BaseModel<UserRole>
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}
