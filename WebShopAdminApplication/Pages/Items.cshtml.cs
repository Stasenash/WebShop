using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebShopAdminApplication.Pages
{
    public class ItemsModel : PageModel
    {
        private readonly DataService _dataService;

        public List<ItemDto> Items { get; set; }

        public bool IsError { get; set; }

        public ItemsModel(DataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Items = await _dataService.GetItems();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            IsError = await _dataService.DeleteItem(id);
            Items = await _dataService.GetItems();
            return Page();
        }
    }
}
