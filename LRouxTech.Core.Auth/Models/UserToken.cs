public class UserToken
{
    public Guid UserId { get; set; }
    public string TokenValue { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ExpiresOn { get; set; }
    public bool Expired { get; set; } = false;
}
