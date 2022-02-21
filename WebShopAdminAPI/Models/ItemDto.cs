using WebShopAdminAPI.Db;

namespace WebShopAdminAPI.Models
{
    public class ItemDto
    {
        /// <summary>
        /// Id товара
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название товара
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Цена товара
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Ссылка на изображение товара
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Навигационное поле для категории
        /// </summary>
        public string CategoryName { get; set; }

        public ItemDto(Item item)
        {
            Id = item.Id;
            Name = item.Name;
            Price = item.Price;
            ImageUrl = item.ImageUrl;
            CategoryName = item.Category?.Name ?? string.Empty;
        }
    }
}