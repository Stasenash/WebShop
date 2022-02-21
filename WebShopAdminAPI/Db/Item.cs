using System.ComponentModel.DataAnnotations;

namespace WebShopAdminAPI.Db
{
    /// <summary>
    /// Товар
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Id товара
        /// </summary>
        [Key]
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
        public int CategoryId { get; set; }

        /// <summary>
        /// Категория товара
        /// </summary>
        public Category Category { get; set; }
    }
}
