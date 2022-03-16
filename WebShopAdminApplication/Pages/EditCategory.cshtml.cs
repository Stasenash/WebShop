using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace WebShopAdminApplication.Pages
{
    public class EditCategoryModel : PageModel
    {
        private readonly DataService _dataService;

        public EditCategoryModel(DataService dataService)
        {
            _dataService = dataService;
        }

        [BindProperty]
        public CategoryDto Category { get; set; }

        public List<SelectListItem> AllCategories { get; set; }

        public bool IsSuccess { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var categories = await _dataService.GetCategories();
            List<int> excludedChildIds = new List<int> { id };
            excludedChildIds.AddRange(categories.Where(c => c.Id != null && c.ParentId == id).Select(c => c.Id.Value).ToList());
            
            while (true)
            {
                int count1 = excludedChildIds.Count;
                excludedChildIds.AddRange(
                    categories
                    .Where(c => 
                        c.Id != null 
                        && c.ParentId != null 
                        && excludedChildIds.Contains(c.ParentId.Value)
                        && !excludedChildIds.Contains(c.Id.Value))
                    .Select(c => c.Id.Value)
                    .ToList());
                int count2 = excludedChildIds.Count;
                if (count1 == count2) break;
            }

            AllCategories = categories.Where(x => !excludedChildIds.Contains(x.Id.Value)).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();

            Category = categories.FirstOrDefault(x => x.Id == id) ?? new CategoryDto();

            AllCategories.Insert(0, new SelectListItem { Value = "null", Text = string.Empty });

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Category.Id == null)
                IsSuccess = await _dataService.CreateCategory(Category);
            else
                IsSuccess = await _dataService.UpdateCategory(Category);

            return RedirectToPage("Categories");
        }
    }
}
