using Microsoft.AspNetCore.Mvc;
using WebShopAdminAPI.Db;
using WebShopAdminAPI.Models;

namespace WebShopAdminAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly AdminDbContext _db;

        public CategoryController(AdminDbContext db, ILogger<CategoryController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet(Name = "create")]
        public CategoryDto CreateCategory(string name)
        {
            Category category = new Category
            {
                Name = name
            };

            _db.Categories.Add(category);
            _db.SaveChanges();

            var created = _db.Categories.FirstOrDefault(x => x.Name == name);

            return new CategoryDto
            {
                Id = created?.Id ?? -1,
                Name = created?.Name ?? string.Empty
            };
        }
    }
}