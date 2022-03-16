using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebShopCatalogApplication.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DataService _dataService;

        public Category Category { get; set; }

        public IndexModel(DataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IActionResult> OnGetAsync(int? categoryId)
        {
            Category = await _dataService.GetCatalog(categoryId);
            return Page();
        }
    }
}