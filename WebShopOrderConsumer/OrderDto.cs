using WebShopContracts;
using WebShopOrderConsumer.Db;

namespace WebShopOrderConsumer
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
