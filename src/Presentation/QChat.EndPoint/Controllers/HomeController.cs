using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QChat.Application.Interfaces;
using QChat.Application.Services.Chats.Commands;
using QChat.Application.Services.Users.Queries;
using QChat.EndPoint.Models;
using QChat.EndPoint.Services;
using System.Diagnostics;

namespace QChat.EndPoint.Controllers;
[Authorize]
public class HomeController : Controller
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public HomeController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Chats = await _mediator.Send(new GetChatsQuery(_currentUserService.UserId));
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
    public async Task<IActionResult> AddUserToGroup(AddUserToGroupCommand command)
    {
        var result = await _mediator.Send(command);
        return result.ToJson();
    }
}
