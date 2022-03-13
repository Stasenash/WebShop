using Microsoft.AspNetCore.Mvc;

namespace WebShopAdminAPI.Models
{
    [BindProperties]
    public class CategoryUpdateRequest : CategoryCreateRequest
    {
        public int Id { get; set; }
    }
}
