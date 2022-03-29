using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using WebShopContracts;

namespace WebShopAdminApplication
{
    public class CategoryDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }

    public class ItemDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
    }

    public class DataService
    {
        private IConfigurationRoot _config;

        public DataService(IConfiguration configRoot)
        {
            _config = (IConfigurationRoot)configRoot;
        }

        #region Categories
        public async Task<List<CategoryDto>> GetCategories()
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync("/categories/list");
                var content = await response.Content.ReadAsStringAsync();

                var categories = JsonConvert.DeserializeObject<List<CategoryDto>>(content) ?? new List<CategoryDto>();
                return categories;
            }
        }

        public async Task<bool> CreateCategory(CategoryDto category)
        {
            category.Id = null;
            using (var httpClient = CreateHttpClient())
            {
                var content = GetHttpContent(JsonConvert.SerializeObject(category));
                var response = await httpClient.PostAsync("/categories/create", content);
                return response.StatusCode == HttpStatusCode.OK;
            }
        }

        public async Task<bool> UpdateCategory(CategoryDto category)
        {
            using (var httpClient = CreateHttpClient())
            {
                var content = GetHttpContent(JsonConvert.SerializeObject(category));
                var response = await httpClient.PutAsync("/categories/edit", content);
                return response.StatusCode == HttpStatusCode.OK;
            }
        }

        public async Task<bool> DeleteCategory(int id)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.DeleteAsync($"/categories/delete?id={id}");
                return response.StatusCode == HttpStatusCode.OK;
            }
        }
        #endregion

        #region Items
        public async Task<List<ItemDto>> GetItems()
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync("/items/list");
                var content = await response.Content.ReadAsStringAsync();

                var items = JsonConvert.DeserializeObject<List<ItemDto>>(content) ?? new List<ItemDto>();
                return items;
            }
        }

        public async Task<ItemDto> GetItem(int id)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync($"/items?id={id}");
                var content = await response.Content.ReadAsStringAsync();

                var item = JsonConvert.DeserializeObject<ItemDto>(content) ?? new ItemDto();
                return item;
            }
        }

        public async Task<bool> CreateItem(ItemDto item)
        {
            item.Id = null;
            using (var httpClient = CreateHttpClient())
            {
                var content = GetHttpContent(JsonConvert.SerializeObject(item));
                var response = await httpClient.PostAsync("/items/create", content);
                return response.StatusCode == HttpStatusCode.OK;
            }
        }

        public async Task<bool> UpdateItem(ItemDto item)
        {
            using (var httpClient = CreateHttpClient())
            {
                var content = GetHttpContent(JsonConvert.SerializeObject(item));
                var response = await httpClient.PutAsync("/items/edit", content);
                return response.StatusCode == HttpStatusCode.OK;
            }
        }

        public async Task<bool> DeleteItem(int id)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.DeleteAsync($"/items/delete?id={id}");
                return response.StatusCode == HttpStatusCode.OK;
            }
        }
        #endregion

        public async Task<(string, string)> Auth(string username, string password)
        {
            using (var httpClient = CreateHttpClient())
            {
                var content = GetHttpContent(JsonConvert.SerializeObject(new { UserName = username, Password = password }));
                var response = await httpClient.PostAsync($"/auth", content);
                var reponseContent = await response.Content.ReadAsStringAsync();
                var jo = JObject.Parse(reponseContent);

                var token = jo["token"]?.ToString();
                var message = jo["message"]?.ToString();                

                return (token, message);
            }
        }

        public async Task<(bool, string)> Register(string username, string password)
        {
            using (var httpClient = CreateHttpClient())
            {
                var content = GetHttpContent(JsonConvert.SerializeObject(new { UserName = username, Password = password }));
                var response = await httpClient.PostAsync($"/register", content);

                var reponseContent = await response.Content.ReadAsStringAsync();
                var jo = JObject.Parse(reponseContent);
                var message = jo["message"]?.ToString();

                return (response.StatusCode == HttpStatusCode.OK, message);
            }
        }

        public async Task<List<OrderDto>> GetOrders()
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync($"/orders");
                var content = await response.Content.ReadAsStringAsync();

                var orders = JsonConvert.DeserializeObject<List<OrderDto>>(content);
                return orders;
            }
        }

        public async Task<bool> CancelOrder(int orderId)
        {
            using (var httpClient = CreateHttpClient())
            {
                var content = GetHttpContent(JsonConvert.SerializeObject(new { OrderId = orderId, NewStatus = OrderStatus.AdminCancelled }));
                var response = await httpClient.PostAsync($"/orders/changeStatus", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return response.StatusCode == HttpStatusCode.OK;
            }
        }

        public async Task<bool> ApproveOrder(int orderId)
        {
            using (var httpClient = CreateHttpClient())
            {
                var content = GetHttpContent(JsonConvert.SerializeObject(new { OrderId = orderId, NewStatus = OrderStatus.AdminApproved }));
                var response = await httpClient.PostAsync($"/orders/changeStatus", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                return response.StatusCode == HttpStatusCode.OK;
            }
        }

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            string adminApiBaseUrl = _config.GetConnectionString("WebShopAdminApiBaseUrl");
            httpClient.BaseAddress = new Uri(adminApiBaseUrl);

            var token = HttpUtils.GetToken();
            if (!string.IsNullOrEmpty(token)) httpClient.DefaultRequestHeaders.Add("Authorization", token);
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
