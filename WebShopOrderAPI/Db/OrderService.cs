using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShopContracts;
using WebShopOrderAPI.Models;

namespace WebShopOrderAPI.Db
{
    public class OrderService
    {
        private readonly OrderDbContext _dbContext;
        private readonly IConfigurationRoot _config;

        public OrderService(OrderDbContext dbContext, IConfiguration configRoot)
        {
            _dbContext = dbContext;
            _config = (IConfigurationRoot)configRoot;
        }

        public async Task<OrderDto> CreateOrder(int userId, List<int> itemIds)
        {
            var order = new Order
            {
                UserId = userId,
                CreatedAt = DateTime.Now,
                LastUpdatedAt = DateTime.Now,
                Status = OrderStatus.Created,
                Items = JsonConvert.SerializeObject(itemIds)
            };

            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            return new OrderDto(order);
        }

        internal List<OrderDto> GetAllOrders()
        {
            var allOrders = _dbContext.Orders.Select(x => new OrderDto(x)).ToList();
            return allOrders;
        }

        internal List<OrderDto> GetUserOrders(int userId)
        {
            var allOrders = _dbContext.Orders.Where(x => x.UserId == userId).Select(x => new OrderDto(x)).ToList();
            return allOrders;
        }

        internal int? GetOrderOwnerId(int orderId)
        {
            return _dbContext.Orders.Where(x => x.Id == orderId).FirstOrDefault()?.UserId;
        }

        internal async Task ChangeOrderStatus(int userId, int orderId, OrderStatus newStatus)
        {
            var order = _dbContext.Orders.Where(x => x.Id == orderId).FirstOrDefault();
            if (order == null) throw new ArgumentNullException($"Couldn't find order with id='{orderId}'");
            order.Status = newStatus;
            order.LastUpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
        }

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            string adminApiBaseUrl = _config.GetConnectionString("AdminApiBaseUrl");
            httpClient.BaseAddress = new Uri(adminApiBaseUrl);

            return httpClient;
        }
    }
}
