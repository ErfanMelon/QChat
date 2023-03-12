#nullable disable
namespace QChat.Domain.Entities;

public class User:BaseEntity
{
    public string UserName { get; set; }
    public string Password { get; set; }

    public virtual ICollection<UserChat> UserChats { get; set; }
}