using WebShopContracts;

namespace WebShopOrderAPI.Models
{
    public class OrderCreated
    {
        public List<int> Items { get; set; }
    }

    public class OrderStatusChanged
    {
        public int OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
    }
}
