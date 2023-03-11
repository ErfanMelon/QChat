using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QChat.EndPoint.Pages;
[Authorize]
public class IndexModel : PageModel
{
    public void OnGet() { }
}