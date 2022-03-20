using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace WebShopAdminGateway
{
    public class Category
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }

    public class Item
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
    }

    public class AdminService
    {
        private IConfigurationRoot _config;

        public AdminService(IConfiguration configRoot)
        {
            _config = (IConfigurationRoot)configRoot;
        }

        #region Categories
        public async Task<List<Category>> GetCategories()
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync("/categories/list");
                var content = await response.Content.ReadAsStringAsync();

                var categories = JsonConvert.DeserializeObject<List<Category>>(content) ?? new List<Category>();
                return categories;
            }
        }

        public async Task<bool> CreateCategory(Category category)
        {
            category.Id = null;
            using (var httpClient = CreateHttpClient())
            {
                var content = GetHttpContent(JsonConvert.SerializeObject(category));
                var response = await httpClient.PostAsync("/categories/create", content);
                return response.StatusCode == HttpStatusCode.OK;
            }
        }

        public async Task<bool> UpdateCategory(Category category)
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
        public async Task<List<Item>> GetItems()
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync("/items/list");
                var content = await response.Content.ReadAsStringAsync();

                var items = JsonConvert.DeserializeObject<List<Item>>(content) ?? new List<Item>();
                return items;
            }
        }

        public async Task<Item> GetItem(int id)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync($"/items?id={id}");
                var content = await response.Content.ReadAsStringAsync();

                var item = JsonConvert.DeserializeObject<Item>(content) ?? new Item();
                return item;
            }
        }

        public async Task<bool> CreateItem(Item item)
        {
            item.Id = null;
            using (var httpClient = CreateHttpClient())
            {
                var content = GetHttpContent(JsonConvert.SerializeObject(item));
                var response = await httpClient.PostAsync("/items/create", content);
                return response.StatusCode == HttpStatusCode.OK;
            }
        }

        public async Task<bool> UpdateItem(Item item)
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


        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            string adminApiBaseUrl = _config.GetConnectionString("WebShopAdminApiBaseUrl");
            httpClient.BaseAddress = new Uri(adminApiBaseUrl);
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
