using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebShopCatalogApplication.Pages
{
    public class BasketModel : PageModel
    {
        private readonly DataService _dataService;

        public List<BasketItem> Items { get; set; }
        public double TotalPrice { get; set; }

        public BasketModel(DataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!HttpUtils.GetIsAuth()) return RedirectToPage("Auth");

            var basket = await _dataService.GetBasket();
            Items = basket?.Items;
            TotalPrice = basket?.TotalPrice ?? 0;
            return Page();
        }

        public async Task<IActionResult> OnGetDeleteItemAsync(int itemId)
        {
            if (!HttpUtils.GetIsAuth()) return RedirectToPage("Auth");

            var basket = await _dataService.BasketDeleteItem(itemId);
            Items = basket?.Items;
            TotalPrice = basket?.TotalPrice ?? 0;
            return Page();
        }

        public async Task<IActionResult> OnGetAddItemAsync(int itemId)
        {
            if (!HttpUtils.GetIsAuth()) return RedirectToPage("Auth");

            var basket = await _dataService.BasketAddItem(itemId);
            Items = basket?.Items;
            TotalPrice = basket?.TotalPrice ?? 0;
            return Page();
        }
    }
}
