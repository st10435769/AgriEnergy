using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication3.Pages
{
    public class DashboardModel : PageModel
    {
        public string UserName { get; set; }
        public void OnGet()
        {
        }
    }
}
