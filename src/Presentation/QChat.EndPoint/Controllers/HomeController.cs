using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QChat.Application.Interfaces;
using QChat.Application.Services.Chats.Commands;
using QChat.Application.Services.Chats.Queries;
using QChat.Application.Services.Messages.Commands;
using QChat.Application.Services.Users.Queries;
using QChat.Domain.Entities;
using QChat.EndPoint.Hubs;
using QChat.EndPoint.Models;
using QChat.EndPoint.Services;
using System.Diagnostics;

namespace QChat.EndPoint.Controllers;
[Authorize]
public class HomeController : Controller
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;
    private readonly IHubContext<DefaultHub> _hubContext;

    public HomeController(IMediator mediator, ICurrentUserService currentUserService, IHubContext<DefaultHub> hubContext)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _hubContext = hubContext;
    }

    public async Task<IActionResult> Index()
    {
        var chats = await _mediator.Send(new GetChatsQuery(_currentUserService.UserId));
        ViewBag.Chats = chats;

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    [HttpPost]
    public async Task<IActionResult> CreateGroup(NewChatCommand command)
    {
        command.UserId = _currentUserService.UserId;
        var result = await _mediator.Send(command);
        return result.ToJson();
    }
    [HttpPost]
    public async Task<IActionResult> AddUserToGroup(AddUserToGroupCommand command)
    {
        var result = await _mediator.Send(command);
        return result.ToJson();
    }
    [HttpPost]
    public async Task<IActionResult> GetChat(long? chatId, long? oldChatId, string? connectionId)
    {
        var query = new GetChatQuery(chatId, _currentUserService.UserId);
        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            await _hubContext.Groups
                .RemoveFromGroupAsync(connectionId, oldChatId.Value.ToString());
            await _hubContext.Groups
                    .AddToGroupAsync(connectionId, chatId.Value.ToString());
        }

        return result.ToJson();
    }
    [HttpPost]
    public async Task<IActionResult> GetChatInfo(long? chatId)
    {
        var query = new GetChatDetailQuery(chatId, _currentUserService.UserId);
        var result = await _mediator.Send(query);
        return result.ToJson();
    }
    [HttpPost]
    public async Task<IActionResult> CreatePrivateChat(NewPrivateChatCommand command)
    {
        command.UserId = _currentUserService.UserId;
        var result = await _mediator.Send(command);
        return result.ToJson();
    }
    [HttpPost]
    public async Task<IActionResult> SendMedia(NewMediaMessageCommand command)
    {
        command.Media = Request.Form.Files.FirstOrDefault();
        command.UserId = _currentUserService.UserId;
        var result = await _mediator.Send(command);
        if (result.HasValue)
        {
            await _hubContext.Clients.Group(command.ChatId).SendAsync("UpdateChat", command.ChatId);
            await _hubContext.Clients.Users(result.Value).SendAsync("SendNotification", _currentUserService.Name, command.ChatId, command.Media.FileName);
        }
        return Json("OK");
    }
}
