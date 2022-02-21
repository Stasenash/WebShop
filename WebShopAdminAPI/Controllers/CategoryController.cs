using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShopAdminAPI.Db;
using WebShopAdminAPI.Models;

namespace WebShopAdminAPI.Controllers
{
    [ApiController]
    [ApiExplorerSettings]
    [Route("categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly AdminDbContext _db;

        public CategoryController(AdminDbContext db, ILogger<CategoryController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet("list")]
        public ActionResult GetAllCategories()
        {
            try
            {
                var categories = _db.Categories
                    .Include(x => x.SubCategories)
                    .Select(x => new CategoryDto(x))
                    .ToList();

                return Ok(categories);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("create")]
        public ActionResult CreateCategory([FromBody] CategoryCreateRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Name))
            {
                return BadRequest("Запрос не может быть пустым");
            }

            try
            {
                if (_db.Categories.Any(x => x.Name == request.Name))
                {
                    return BadRequest("Категория с таким именем уже существует");
                }

                Category category = new Category
                {
                    Name = request.Name,
                    ParentId = request.ParentId
                };

                _db.Categories.Add(category);
                _db.SaveChanges();

                var created = _db.Categories.FirstOrDefault(x => x.Name == request.Name);
                return Ok(new CategoryDto(created));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("edit")]
        public ActionResult CreateCategory([FromBody] CategoryEditRequest request)
        {
            if (request == null || request.Id == 0 || string.IsNullOrEmpty(request.Name))
            {
                return BadRequest("Запрос не может быть пустым");
            }

            try
            {
                var category = _db.Categories.FirstOrDefault(x => x.Id == request.Id);
                if (category == null)
                {
                    return BadRequest($"Категории с Id {request.Id} не найдено");
                }

                category.Name = request.Name;
                category.ParentId = request.ParentId;
                _db.SaveChanges();

                var edited = _db.Categories.FirstOrDefault(x => x.Id == request.Id);
                return Ok(new CategoryDto(edited));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteCategory(int id)
        {
            if (id == 0)
            {
                return BadRequest("Запрос не может быть пустым");
            }

            try
            {
                var category = _db.Categories.FirstOrDefault(x => x.Id == id);
                _db.Categories.Remove(category);
                _db.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}