using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebShopBasketAPI.Db;
using WebShopBasketAPI.Models;
using WebShopContracts;

namespace WebShopBasketAPI.Controllers
{
    [ApiController]
    [ApiExplorerSettings]
    [Route("basket")]
    public class BasketController : ControllerBase
    {
        private readonly ILogger<BasketController> _logger;
        private readonly BasketService _basketDb;
        private readonly AdminService _dataService;
        readonly IPublishEndpoint _publishEndpoint;

        public BasketController(
            BasketService db,
            ILogger<BasketController> logger,
            AdminService dataService,
            IPublishEndpoint publishEndpoint)
        {
            _basketDb = db;
            _logger = logger;
            _dataService = dataService;
            _publishEndpoint = publishEndpoint;
        }

        private (bool, User) CheckUserRole(string userHeader)
        {
            if (string.IsNullOrEmpty(userHeader))
                return (false, null);

            User user = JsonConvert.DeserializeObject<User>(userHeader);
            if (user == null)
                return (false, null);

            return (user.Roles.Any(x => x.Name == "User"), user);
        }

        [HttpGet("")]
        public ActionResult GetBasket([FromHeader(Name = "User")] string userHeader)
        {
            (var isAuthed, var user) = CheckUserRole(userHeader);
            if (!isAuthed) return StatusCode(StatusCodes.Status401Unauthorized);

            try
            {
                var basket = _basketDb.Get(user.Id);
                return Ok(basket);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("add/{itemId}")]
        public async Task<ActionResult> AddItem(int itemId, [FromHeader(Name = "User")] string userHeader)
        {
            (var isAuthed, var user) = CheckUserRole(userHeader);
            if (!isAuthed) return StatusCode(StatusCodes.Status401Unauthorized);

            try
            {
                var item = await _dataService.GetItem(itemId);
                _basketDb.AddItem(user.Id, item);
                var basket = _basketDb.Get(user.Id);
                return Ok(basket);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("delete/{itemId}")]
        public ActionResult DeleteItem(int itemId, [FromHeader(Name = "User")] string userHeader)
        {
            (var isAuthed, var user) = CheckUserRole(userHeader);
            if (!isAuthed) return StatusCode(StatusCodes.Status401Unauthorized);

            try
            {
                _basketDb.DeleteItem(user.Id, itemId);
                var basket = _basketDb.Get(user.Id);
                return Ok(basket);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("createOrder")]
        public async Task<ActionResult> CreateOrder([FromHeader(Name = "User")] string userHeader)
        {
            (var isAuthed, var user) = CheckUserRole(userHeader);
            if (!isAuthed) return StatusCode(StatusCodes.Status401Unauthorized);

            try
            {
                var basket = _basketDb.Get(user.Id);
                await _publishEndpoint.Publish<IOrderCreated>(new
                {
                    User = user,
                    Items = basket.Items.Select(x => x.Id).ToList()
                });
                _basketDb.Clear(user.Id);

                return Ok(basket);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}