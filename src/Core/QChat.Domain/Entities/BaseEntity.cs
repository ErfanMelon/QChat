namespace QChat.Domain.Entities;

public abstract class BaseEntity<TKey>: BaseEntityKeyLess
{
    public TKey Id { get; set; }
    // DateTime CreatedDate { get; set; } = DateTime.Now;
    // DateTime? UpdateDate { get; set; }
    // DateTime? RemoveDate { get; set; }
    // bool IsRemoved { get; set; } = false;
}
public abstract class BaseEntity : BaseEntity<Guid> { }
public abstract class BaseEntityKeyLess { }
