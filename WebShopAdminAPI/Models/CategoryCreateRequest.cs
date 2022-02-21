using Microsoft.AspNetCore.Mvc;

namespace WebShopAdminAPI.Models
{
    [BindProperties]
    public class CategoryCreateRequest
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }
}
