using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebShopContracts;
using WebShopOrderAPI.Db;
using WebShopOrderAPI.Models;

namespace WebShopOrderAPI.Controllers
{
    [ApiController]
    [ApiExplorerSettings]
    [Route("orders")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly OrderService _orderService;
        readonly IPublishEndpoint _publishEndpoint;

        public OrderController(
            OrderService orderService,
            ILogger<OrderController> logger,
            IPublishEndpoint publishEndpoint)
        {
            _orderService = orderService;
            _logger = logger;
        }

        private User GetUser(string userHeader)
        {
            if (string.IsNullOrEmpty(userHeader))
                throw new ArgumentNullException("User in header is null");

            User user = JsonConvert.DeserializeObject<User>(userHeader);
            if (user == null)
                throw new ArgumentNullException("Couldn't deserialize user from header");

            return user;
        }

        [HttpGet("")]
        public ActionResult GetOrders([FromHeader(Name = "User")] string userHeader)
        {
            try
            {
                var user = GetUser(userHeader);
                if (user.Roles.Any(x => x.Name == "Admin"))
                {
                    // return all orders
                    var orders = _orderService.GetAllOrders();
                    return Ok(orders);
                }
                else
                {
                    // return only user orders
                    var orders = _orderService.GetUserOrders(user.Id);
                    return Ok(orders);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("create")]
        public async Task<ActionResult> CreateOrder(
            [FromHeader(Name = "User")] string userHeader, 
            [FromBody] OrderCreated orderCreated)
        {
            try
            {
                var user = GetUser(userHeader);
                var orderId = await _orderService.CreateOrder(user.Id, orderCreated.Items);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("changeStatus")]
        public async Task<ActionResult> ChangeOrderStatus(
            [FromHeader(Name = "User")] string userHeader, 
            [FromBody] OrderStatusChanged orderStatusChanged)
        {
            try
            {
                var user = GetUser(userHeader);
                var ownerId = _orderService.GetOrderOwnerId(orderStatusChanged.OrderId);
                if (!user.Roles.Any(x => x.Name == "Admin") || user.Id != ownerId)
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, new { Message = "You are not allowed to change order status" });
                }

                await _orderService.ChangeOrderStatus(user.Id, orderStatusChanged.OrderId, orderStatusChanged.NewStatus);

                await _publishEndpoint.Publish<IOrderStatusChanged>(new
                {
                    OrderId = orderStatusChanged.OrderId,
                    NewStatus = orderStatusChanged.NewStatus
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