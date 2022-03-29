using WebShopContracts;
using WebShopOrderAPI.Db;

namespace WebShopOrderAPI.Models
{
    public class OrderDto
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }

        public OrderDto() { }

        public OrderDto(Order order)
        {
            Id = order.Id;
            Status = order.Status;
        }
    }
}
