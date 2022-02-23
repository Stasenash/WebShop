using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebShopAdminAPI.Controllers
{
    [ApiController]
    [ApiExplorerSettings]
    [Route("[controller]")]
    public class MainController : ControllerBase
    {
        [HttpGet("Index")]
        public ActionResult Index()
        {
            return Ok("Ok");
        }
    }
}
