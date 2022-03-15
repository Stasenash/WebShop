using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace WebShopCatalogApplication
{
    public class Category
    {
        public string Id { get; set; }
        public int RelationalId { get; set; }
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
        public string Id { get; set; }
        public int RelationalId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
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


        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            string adminApiBaseUrl = _config.GetConnectionString("CatalogApiBaseUrl");
            httpClient.BaseAddress = new Uri(adminApiBaseUrl);
            return httpClient;
        }
    }
}
