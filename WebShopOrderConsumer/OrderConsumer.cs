using MassTransit;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using WebShopContracts;
using WebShopOrderConsumer.Db;

namespace WebShopOrderConsumer
{
    public class OrderConsumer :
        IConsumer<IOrderCreated>,
        IConsumer<IOrderStatusChanged>
    {
        private readonly IConfiguration _config;
        private readonly OrderService _orderService;

        public OrderConsumer(IConfiguration configuration, OrderService orderService)
        {
            _config = configuration;
            _orderService = orderService;
        }

        public async Task Consume(ConsumeContext<IOrderCreated> context)
        {
            Console.WriteLine($"{context.Message.GetType().Name} received");

            var user = context.Message.User;
            var items = context.Message.Items;
            using (var httpClient = CreateHttpClient(user))
            {
                var content = GetHttpContent(JsonConvert.SerializeObject(new { Items = items }));
                var response = await httpClient.PostAsync($"orders/create", content);

                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Couldn't consume {context.Message.GetType().Name}");

                var responseContent = await response.Content.ReadAsStringAsync();
                var orderDto = JsonConvert.DeserializeObject<OrderDto>(responseContent);

                await _orderService.CreateAggregate(user.Id, orderDto.Id);
            }
        }

        public async Task Consume(ConsumeContext<IOrderStatusChanged> context)
        {
            var user = context.Message.User;
            var newStatus = context.Message.NewStatus;
            var orderId = context.Message.OrderId;
            using (var httpClient = CreateHttpClient(user))
            {
                var content = GetHttpContent(JsonConvert.SerializeObject(new { NewStatus = newStatus, OrderId = orderId}));
                var response = await httpClient.PostAsync($"orders/changeStatus", content);

                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Couldn't consume {context.Message.GetType().Name}");

                await _orderService.CreateAggregate(user.Id, orderId);
            }

        }

        private HttpClient CreateHttpClient(User user)
        {
            var httpClient = new HttpClient();
            string orderApiBaseUrl = _config.GetConnectionString("OrderApiBaseUrl");
            httpClient.BaseAddress = new Uri(orderApiBaseUrl);
            httpClient.DefaultRequestHeaders.Add("User", JsonConvert.SerializeObject(user));

            return httpClient;
        }

        private HttpContent GetHttpContent(string json)
        {
            var content = new StringContent(json);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            return content;
        }
    }
}
