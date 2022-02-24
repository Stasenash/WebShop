using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebShopAdminApplication.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private IConfigurationRoot _config;

        public string Content { get; set; }

        public IndexModel(
            ILogger<IndexModel> logger,
            IConfiguration configRoot)
        {
            _logger = logger;
            _config = (IConfigurationRoot)configRoot;
        }

        public async Task OnGet()
        {
            var httpClient = new HttpClient();
            string adminApiBaseUrl = _config.GetConnectionString("WebShopAdminApiBaseUrl");
            httpClient.BaseAddress = new Uri(adminApiBaseUrl);
            var response = await httpClient.GetAsync("/categories/list");
            var content = await response.Content.ReadAsStringAsync();
            Content = content;
        }
    }
}