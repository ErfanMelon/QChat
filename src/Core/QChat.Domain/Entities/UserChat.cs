namespace QChat.Domain.Entities;

public class UserChat:BaseEntityKeyLess
{
    public long ChatId { get; set; }
    public virtual Chat Chat { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
}