using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebShopAdminApplication.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            var isAuth = HttpUtils.GetIsAuth(HttpContext);
            if (!isAuth) return RedirectToPage("Auth");

            return Page();
        }
    }
}