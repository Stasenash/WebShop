using System.ComponentModel.DataAnnotations;

namespace WebShopAdminAPI.Db
{
    /// <summary>
    /// Категория товаров
    /// </summary>
    public class Category
    {
        [Key]
        /// <summary>
        /// Id категории
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название категории
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Список товаров в категории
        /// </summary>
        public List<int> ItemId { get; set; }

        /// <summary>
        /// Родительская категория
        /// </summary>
        public Category Parent { get; set; }

        /// <summary>
        /// Навигационное поле для Parent
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// Дочерние подкатегории
        /// </summary>
        public ICollection<Category> SubCategories { get; } = new List<Category>();

        /// <summary>
        /// Товары в категории
        /// </summary>
        public ICollection<Item> Items { get; } = new List<Item>();
    }
}
