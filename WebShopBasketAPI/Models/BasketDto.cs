using WebShopBasketAPI.Db;

namespace WebShopBasketAPI.Models
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public double TotalPrice { get; set; }
    }

    public class BasketDto
    {
        public List<ItemDto> Items { get; set; } = new List<ItemDto>();
        public double TotalPrice {  get; set; }

        public BasketDto() { }

        public BasketDto(Basket basket)
        {
            Items = basket.Items.Select(x => new ItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                Count = x.Count,
                TotalPrice = x.Price * x.Count
            }).ToList();

            TotalPrice = Items.Sum(x => x.TotalPrice);
        }
    }
}
