using Microsoft.AspNetCore.SignalR;
using QChat.Application.Interfaces;

namespace QChat.EndPoint.Hubs;

public class DefaultHub : Hub
{
    private readonly ICurrentUserService _currentUserService;
    public DefaultHub(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", $"Hello {_currentUserService.Name}");
        await base.OnConnectedAsync();
    }
}
