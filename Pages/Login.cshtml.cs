using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApplication3.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace WebApplication3.Pages
{
    public class LoginModel : PageModel
    {

        [BindProperty]
        public string username { get; set; }

        [BindProperty]
        public string password { get; set; }

        [BindProperty]
        public string role { get; set; }

        [BindProperty]
        public string ErrorMessage { get; set; }


        public LoginModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private readonly string _connectionString;

        public void OnGet()
        {
            
        }

        public IActionResult OnPost()
        {
            if (IsValidUser(username, password, role))
            {
                if (role == "Farmer")
                {
                    return RedirectToPage("/FarmerDashboard");
                }
                else if (role == "Employee")
                {
                    return RedirectToPage("/EmployeeDashboard");
                }
            }

            ErrorMessage = "Invalid Username, Password or Role";
            return Page();
        }

        private bool IsValidUser(string username, string password, string role)
        {
            string tableName = role == "Farmer" ? "farmers" : "employees";
            string query = $"SELECT COUNT(*) FROM {tableName} WHERE username = @username AND password = @password AND roles = @roles";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    cmd.Parameters.AddWithValue("@roles", role);

                    int count = (int)cmd.ExecuteScalar();
                    bool isValid = count > 0;

                    return isValid;
                }
            }
        }

    }
}
