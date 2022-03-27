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

        [BindProperty]
        public bool IsRegistration { get; set; }

        public string Message { get; set; }

        public AuthModel(DataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (IsRegistration)
            {
                (bool isRegistered, string errorMessage) = await _dataService.Register(Username, Password);

                IsRegistration = !isRegistered;
                Message = errorMessage;

                return Page();
            }
            else
            {
                (string token, string errorMessage) = await _dataService.Auth(Username, Password);
                if (string.IsNullOrEmpty(token))
                {
                    Message = errorMessage;
                    return Page();
                }

                HttpUtils.SetIsAuth();
                HttpUtils.SetToken(token);
                return RedirectToPage("Index");
            }
        }

        public IActionResult OnGetLogout()
        {
            HttpUtils.SetIsAuth(false);
            HttpUtils.ResetToken();
            return Page();
        }
    }
}
