using MediatR;
using Microsoft.AspNetCore.SignalR;
using QChat.Application.Interfaces;
using QChat.Application.Services.Chats.Commands;
using QChat.Application.Services.Chats.Queries;
using QChat.EndPoint.Services;

namespace QChat.EndPoint.Hubs;

public class DefaultHub : Hub
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;
    public DefaultHub(ICurrentUserService currentUserService, IMediator mediator)
    {
        _currentUserService = currentUserService;
        _mediator = mediator;
    }
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", _currentUserService.Name,Context.ConnectionId);
        await base.OnConnectedAsync();
    }
    public async Task NewGroup(NewChatCommand command)
    {
        command.UserId = _currentUserService.UserId;
        var result = await _mediator.Send(command);
        await Clients.Caller.SendAsync("NewGroup", result.ToJson().Value);
    }
    public async Task SendMessage(string msg, string chatId)
    {
        var message = new NewMessageCommand(_currentUserService.UserId, chatId, msg);
        await _mediator.Send(message);
        await Clients.Group(chatId.ToString()).SendAsync("UpdateChat", chatId);
    }
}