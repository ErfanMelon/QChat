namespace QChat.Domain.Entities;

public class Message:BaseEntity
{
    public virtual User User { get; set; }
    public DateTime PostDate { get; set; }
    public string Content { get; set; }
    public virtual Chat Chat { get; set; }
    public long ChatId { get; set; }
}
