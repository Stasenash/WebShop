using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebShopContracts;

namespace WebShopCatalogApplication.Pages
{
    public class OrdersModel : PageModel
    {
        private readonly DataService _dataService;

        public List<OrderDto> Orders { get; set; }

        public OrdersModel(DataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!HttpUtils.GetIsAuth()) return RedirectToPage("Auth");

            var orders = await _dataService.GetOrders();
            Orders = orders;
            return Page();
        }

        public async Task<IActionResult> OnGetCancelAsync(int orderId)
        {
            if (!HttpUtils.GetIsAuth()) return RedirectToPage("Auth");

            await _dataService.CancelOrder(orderId);
            var orders = await _dataService.GetOrders();
            Orders = orders;
            return Page();
        }
    }
}
