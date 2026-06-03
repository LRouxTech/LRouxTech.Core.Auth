namespace LRouxTech.Core.Auth.Core.Entities;

public class UserToken
{
    public Guid UserTokenId { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    public string TokenValue { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresOn { get; set; }
    public bool Expired { get; set; } = false;
}