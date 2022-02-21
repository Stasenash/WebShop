using Microsoft.AspNetCore.Mvc;

namespace WebShopAdminAPI.Models
{
    [BindProperties]
    public class ItemCreateRequest
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
    }
}