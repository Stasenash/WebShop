using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebShopAdminApplication.Pages
{
    public class AuthModel : PageModel
    {
        private readonly DataService _dataService;

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string Message { get; set; }

        public AuthModel(DataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = await _dataService.Auth(Username, Password);
            if (string.IsNullOrEmpty(token))
            {
                Message = "Not registered or wrong password";
                return Page();
            }

            HttpUtils.SetIsAuth(HttpContext);
            return RedirectToPage("Index");
        }
    }
}
