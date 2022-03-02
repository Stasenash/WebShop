using WebShopAdminAPI.Db;

namespace WebShopAdminAPI.Models
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }

        public CategoryDto(Category category)
        {
            Id = category?.Id ?? -1;
            Name = category?.Name ?? string.Empty;
            ParentId = category.ParentId;
        }
    }
}
