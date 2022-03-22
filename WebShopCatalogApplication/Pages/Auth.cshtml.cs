using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebShopCatalogApplication.Pages
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

                if (!isRegistered) Message = errorMessage;
                else IsRegistration = false;

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
    }
}
