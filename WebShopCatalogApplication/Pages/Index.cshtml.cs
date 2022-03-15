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

        public async Task<Category> OnGet(int? categoryId)
        {
            return await _dataService.GetCatalog(categoryId);
        }
    }
}