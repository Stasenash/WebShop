using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebShopAdminAPI.Db;
using WebShopAdminAPI.Models;
using WebShopContracts;

namespace WebShopAdminAPI.Controllers
{
    [ApiController]
    [ApiExplorerSettings]
    [Route("categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly AdminDbContext _db;
        readonly IPublishEndpoint _publishEndpoint;

        public CategoryController(
            AdminDbContext db, 
            ILogger<CategoryController> logger,
            IPublishEndpoint publishEndpoint)
        {
            _db = db;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        private bool CheckUserRole(string userHeader)
        {
            if (string.IsNullOrEmpty(userHeader))
                return false;

            User user = JsonConvert.DeserializeObject<User>(userHeader);
            if (user == null)
                return false;

            return user.Roles.Any(x => x.Name == "Admin");
        }

        [HttpGet("list")]
        public ActionResult GetAllCategories([FromHeader(Name = "User")] string userHeader)
        {
            if (!CheckUserRole(userHeader))
                return StatusCode(StatusCodes.Status401Unauthorized);

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
        public ActionResult CreateCategory([FromBody] CategoryCreateRequest request, [FromHeader(Name = "User")] string userHeader)
        {
            if (!CheckUserRole(userHeader))
                return StatusCode(StatusCodes.Status401Unauthorized);

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
                
                _publishEndpoint.Publish<ICategoryCreated>(new 
                {  
                    Id = created.Id,
                    Name = created.Name,
                    ParentId = created.ParentId
                });

                return base.Ok(new Models.CategoryDto(created));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("edit")]
        public ActionResult UpdateCategory([FromBody] CategoryUpdateRequest request, [FromHeader(Name = "User")] string userHeader)
        {
            if (!CheckUserRole(userHeader))
                return StatusCode(StatusCodes.Status401Unauthorized);

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

                var updated = _db.Categories.FirstOrDefault(x => x.Id == request.Id);

                _publishEndpoint.Publish<ICategoryUpdated>(new
                {
                    Id = updated.Id,
                    Name = updated.Name,
                    ParentId = updated.ParentId
                });

                return base.Ok(new Models.CategoryDto(updated));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteCategory(int id, [FromHeader(Name = "User")] string userHeader)
        {
            if (!CheckUserRole(userHeader))
                return StatusCode(StatusCodes.Status401Unauthorized);

            if (id == 0)
            {
                return BadRequest("Запрос не может быть пустым");
            }

            try
            {
                var category = _db.Categories.FirstOrDefault(x => x.Id == id);
                _db.Categories.Remove(category);
                _db.SaveChanges();

                _publishEndpoint.Publish<ICategoryDeleted>(new
                {
                    Id = id
                });

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}