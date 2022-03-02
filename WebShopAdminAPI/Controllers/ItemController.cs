using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShopAdminAPI.Db;
using WebShopAdminAPI.Models;

namespace WebShopAdminAPI.Controllers
{
    [ApiController]
    [ApiExplorerSettings]
    [Route("items")] 
    public class ItemController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;
        private readonly AdminDbContext _db;

        public ItemController(AdminDbContext db, ILogger<ItemController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet("list")]
        public ActionResult GetAllItems()
        {
            try
            {
                var items = _db.Items
                    .Include(x => x.Category)
                    .Select(x => new ItemDto(x))
                    .ToList();

                return Ok(items);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("")]
        public ActionResult GetItem(int id)
        {
            try
            {
                var item = _db.Items
                    .Include(x => x.Category)
                    .Where(x => x.Id == id)
                    .Select(x => new ItemDto(x))
                    .FirstOrDefault();

                return Ok(item);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("create")]
        public ActionResult CreateItem([FromBody] ItemCreateRequest request)
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

                Item item = new Item
                {
                    Name = request.Name,
                    Price = request.Price,
                    CategoryId = request.CategoryId,
                    ImageUrl = request.ImageUrl,
                };

                _db.Items.Add(item);
                _db.SaveChanges();

                var created = _db.Items.FirstOrDefault(x => x.Name == request.Name);
                return Ok(new ItemDto(created));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("edit")]
        public ActionResult UpdateItem([FromBody] ItemEditRequest request)
        {
            if (request == null || request.Id == 0 || string.IsNullOrEmpty(request.Name))
            {
                return BadRequest("Запрос не может быть пустым");
            }

            try
            {
                var item = _db.Items.FirstOrDefault(x => x.Id == request.Id);
                if (item == null)
                {
                    return BadRequest($"Категории с Id {request.Id} не найдено");
                }

                item.Name = request.Name;
                item.Price = request.Price;
                item.ImageUrl = request.ImageUrl;
                item.CategoryId = request.CategoryId;

                _db.SaveChanges();

                var edited = _db.Items.FirstOrDefault(x => x.Id == request.Id);
                return Ok(new ItemDto(edited));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("delete")]
        public ActionResult DeleteItem(int id)
        {
            if (id == 0)
            {
                return BadRequest("Запрос не может быть пустым");
            }

            try
            {
                var Item = _db.Items.FirstOrDefault(x => x.Id == id);
                _db.Items.Remove(Item);
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
