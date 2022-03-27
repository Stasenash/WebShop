using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;

namespace WebShopCatalogApplication
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public List<ChildCategory> ChildCategories { get; } = new List<ChildCategory>();
        public List<Item> Items { get; } = new List<Item>();
    }

    public class ChildCategory
    {
        public int Id { get; set; }
        public string Name { set; get; }
    }

    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
    }

    public class Basket
    {
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();
        public double TotalPrice { get; set; }
    }

    public class BasketItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public double TotalPrice { get; set; }
    }

    public class DataService
    {
        private IConfigurationRoot _config;

        public DataService(IConfiguration configRoot)
        {
            _config = (IConfigurationRoot)configRoot;
        }

        public async Task<Category> GetCatalog(int? rootCategory)
        {
            using (var httpClient = CreateHttpClient())
            {
                var uri = !rootCategory.HasValue ? "/catalog" : $"/catalog/{rootCategory}";
                var response = await httpClient.GetAsync(uri);
                var content = await response.Content.ReadAsStringAsync();

                var category = JsonConvert.DeserializeObject<Category>(content);
                return category;
            }
        }

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

        public async Task<Basket> GetBasket()
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync("/basket");
                var content = await response.Content.ReadAsStringAsync();

                var basket = JsonConvert.DeserializeObject<Basket>(content);
                return basket;
            }
        }

        public async Task<Basket> BasketAddItem(int itemId)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync($"/basket/add/{itemId}");
                var content = await response.Content.ReadAsStringAsync();

                var basket = JsonConvert.DeserializeObject<Basket>(content);
                return basket;
            }
        }

        public async Task<Basket> BasketDeleteItem(int itemId)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync($"/basket/delete/{itemId}");
                var content = await response.Content.ReadAsStringAsync();

                var basket = JsonConvert.DeserializeObject<Basket>(content);
                return basket;
            }
        }

        public async Task<Basket> AddItem(int itemId)
        {
            using (var httpClient = CreateHttpClient())
            {
                var response = await httpClient.GetAsync($"/basket/add/{itemId}");
                var content = await response.Content.ReadAsStringAsync();

                var basket = JsonConvert.DeserializeObject<Basket>(content);
                return basket;
            }
        }


        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            string adminApiBaseUrl = _config.GetConnectionString("CatalogApiBaseUrl");
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
