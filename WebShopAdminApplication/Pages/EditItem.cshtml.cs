using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace WebShopAdminApplication.Pages
{
    public class EditItemModel : PageModel
    {
        private readonly DataService _dataService;

        public EditItemModel(DataService dataService)
        {
            _dataService = dataService;
        }

        [BindProperty]
        public ItemDto Item { get; set; }

        public List<SelectListItem> AllCategories { get; set; }

        public bool IsSuccess { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var isAuth = HttpUtils.GetIsAuth();
            if (!isAuth) return RedirectToPage("Auth");

            var categories = await _dataService.GetCategories();
            AllCategories = categories.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();

            Item = id != -1 ? await _dataService.GetItem(id) : new ItemDto();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var isAuth = HttpUtils.GetIsAuth();
            if (!isAuth) return RedirectToPage("Auth");

            if (Item.Id == null)
                IsSuccess = await _dataService.CreateItem(Item);
            else
                IsSuccess = await _dataService.UpdateItem(Item);

            return RedirectToPage("Items");
        }
    }
}
