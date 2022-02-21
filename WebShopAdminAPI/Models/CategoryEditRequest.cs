using Microsoft.AspNetCore.Mvc;

namespace WebShopAdminAPI.Models
{
    [BindProperties]
    public class CategoryEditRequest : CategoryCreateRequest
    {
        public int Id { get; set; }
    }
}
