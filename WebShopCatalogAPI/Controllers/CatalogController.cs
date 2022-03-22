using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebShopCatalogAPI.Db;
using WebShopCatalogAPI.Models;

namespace WebShopCatalogAPI.Controllers
{
    [ApiController]
    [ApiExplorerSettings]
    [Route("catalog")]
    public class CatalogController : ControllerBase
    {
        private readonly ILogger<CatalogController> _logger;
        private readonly CategoryService _db;

        public CatalogController(
            CategoryService db,
            ILogger<CatalogController> logger)
        {
            _db = db;
            _logger = logger;
        }

        private bool CheckUserRole(string userHeader)
        {
            if (string.IsNullOrEmpty(userHeader))
                return false;

            User user = JsonConvert.DeserializeObject<User>(userHeader);
            if (user == null)
                return false;

            return user.Roles.Any(x => x.Name == "User");
        }

        [HttpGet("")]
        public ActionResult GetCatalog([FromHeader(Name = "User")] string userHeader)
        {
            if (!CheckUserRole(userHeader)) return StatusCode(StatusCodes.Status401Unauthorized);

            try
            {
                var categories = _db.Get();
                return Ok(categories);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult GetCategory(int id, [FromHeader(Name = "User")] string userHeader)
        {
            if (!CheckUserRole(userHeader)) return StatusCode(StatusCodes.Status401Unauthorized);

            try
            {
                var categories = _db.Get(id);
                return Ok(categories);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}