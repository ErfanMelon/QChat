using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QChat.EndPoint.Pages.Account;

public class LogoutModel : PageModel
{
    public async Task OnGet()
    {
        if (User.Identity.IsAuthenticated)
            await HttpContext.SignOutAsync();
    }
}
