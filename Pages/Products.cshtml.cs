using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication3.Pages
{
    public class ProductListingModel : PageModel
    {
        public string Products { get; set; }
        public void OnGet()
        {
        }
    }
}
