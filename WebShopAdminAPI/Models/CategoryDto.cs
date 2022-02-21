using WebShopAdminAPI.Db;

namespace WebShopAdminAPI.Models
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();

        public CategoryDto(Category category)
        {
            Id = category?.Id ?? -1;
            Name = category?.Name ?? string.Empty;

            if(category.SubCategories != null && category.SubCategories.Any())
            {
                Categories = category.SubCategories.Select(c => new CategoryDto(c)).ToList();
            }
        }
    }
}
