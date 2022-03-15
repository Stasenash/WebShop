using MassTransit;
using Microsoft.AspNetCore.Mvc;
using WebShopCatalogAPI.Db;

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
            ILogger<CatalogController> logger,
            IPublishEndpoint publishEndpoint)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet("")]
        public ActionResult GetCatalog()
        {
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
        public ActionResult GetCategory(int id)
        {
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