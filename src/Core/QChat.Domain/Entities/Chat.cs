namespace QChat.Domain.Entities;

public class Chat : BaseEntity<long>
{
    public string? Title { get; set; }
    public string? ImageSrc { get; set; }
    public virtual ICollection<UserChat> UserChats { get; set; }
    public virtual ICollection<Message> Messages { get; set; }
}