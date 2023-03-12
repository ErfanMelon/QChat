using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using QChat.Application.Services.Users.Commands;
using QChat.Application.Services.Users.Queries;
using QChat.EndPoint.Services;

namespace QChat.EndPoint.Controllers;

public class AccountController : Controller
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    public AccountController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }
    public IActionResult Index()
    {
        ViewData["ModelState"] = TempData["ModelState"]??default!;
        TempData["Login"] = TempData["Login"] ?? default!;
        TempData["Register"] = TempData["Register"] ?? default!;
        return View();
    }
    [HttpPost("/Register")]
    public async Task<IActionResult> Register(CreateUserCommand command)
    {
        if (!ModelState.IsValid)
        {
            TempData["Register"] = command;
            TempData["ModelState"] = ModelState;
            return View("Index");
        }
        else
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
            {
                result.ToModelState(ModelState);
                TempData["Register"] = command;
                TempData["ModelState"] = ModelState;
                return View("Index");
            }
            return await Login(_mapper.Map<LoginUserCommand>(command));
        }
    }
    [HttpPost("/Login")]
    public async Task<IActionResult> Login(LoginUserCommand command)
    {
        if (!ModelState.IsValid)
        {
            TempData["ModelState"] = ModelState;
            TempData["Login"] = command;
            return View("Index");
        }
        else
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
            {
                result.ToModelState(ModelState);
                TempData["ModelState"] = ModelState;
                TempData["Login"] = command;
                return View("Index");
            }

            var properites = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(10)
            };
            await HttpContext.SignInAsync(result.Value, properites);

            return RedirectToAction("Index", "Home");
        }
    }
    [HttpPost("/Logout")]
    public async Task<IActionResult> Logout()
    {
        if (User.Identity.IsAuthenticated)
            await HttpContext.SignOutAsync();
        return View();
    }
}
