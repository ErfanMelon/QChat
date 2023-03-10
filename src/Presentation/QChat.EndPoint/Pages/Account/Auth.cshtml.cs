using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QChat.Application.Services.Users.Commands;
using QChat.Application.Services.Users.Queries;
using QChat.EndPoint.Services;

namespace QChat.EndPoint.Pages.Account;

public class AuthModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public AuthModel(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    public void OnGet() { }
    public async Task<IActionResult> OnPostRegister(CreateUserCommand command)
    {
        if (!ModelState.IsValid)
        {
            TempData["Register"] = command;
            return Page();
        }
        else
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
            {
                result.ToModelState(ModelState);
                return Page();
            }
            return await OnPostLogin(_mapper.Map<LoginUserCommand>(command));
        }
    }
    public async Task<IActionResult> OnPostLogin(LoginUserCommand command)
    {
        if (!ModelState.IsValid)
        {
            TempData["Login"] = command;
            return Page();
        }
        else
        {
            var result = await _mediator.Send(command);
            if (result.IsFailure)
            {
                result.ToModelState(ModelState);
                return Page();
            }

            var properites = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(10)
            };
            await HttpContext.SignInAsync(result.Value, properites);

            return RedirectToPage("/Index");
        }
    }
}
