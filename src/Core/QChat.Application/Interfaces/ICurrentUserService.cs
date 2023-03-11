namespace QChat.Application.Interfaces;

public interface ICurrentUserService
{
    public string? UserId { get; }
    public string? Name { get; }
}
