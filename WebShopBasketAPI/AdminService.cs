using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using WebShopBasketAPI.Db;

namespace WebShopBasketAPI
{
    public class AdminService
    {
        private readonly IConfigurationRoot _config;

        public AdminService(IConfiguration configRoot)
        {
            _config = (IConfigurationRoot)configRoot;
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

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            string adminApiBaseUrl = _config.GetConnectionString("AdminApiBaseUrl");
            httpClient.BaseAddress = new Uri(adminApiBaseUrl);

            return httpClient;
        }
    }
}
