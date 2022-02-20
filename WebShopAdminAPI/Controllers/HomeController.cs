using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebShopAdminAPI.Controllers
{
    public class HomeController : Controller
    {
        // GET: HomeController
        public ActionResult Index()
        {
            return Ok("Ok");
        }
    }
}
