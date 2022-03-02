using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace WebShopAdminApplication.Pages
{
    public class CategoriesModel : PageModel
    {
        private readonly DataService _dataService;

        public List<CategoryDto> Categories { get; set; }

        public bool IsError { get; set; }

        public CategoriesModel(DataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Categories = await _dataService.GetCategories();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            IsError = await _dataService.DeleteCategory(id);
            Categories = await _dataService.GetCategories();
            return Page();
        }
    }
}
