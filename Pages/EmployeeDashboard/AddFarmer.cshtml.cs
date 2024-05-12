using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;

namespace WebApplication3.Pages.EmployeeDashboard
{
    public class AddProductModel : PageModel
    {

        public AddProductModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private readonly string _connectionString;
        [BindProperty]
        public string username { get; set; }

        [BindProperty]
        public string password { get; set; }

        [BindProperty]
        public string surname { get; set; }

        [BindProperty]
        public string category { get; set; }
        [BindProperty]
        public string name { get; set; }
        [BindProperty]
        public string contact { get; set; }
        [BindProperty]
        public string ErrorMessage { get; set; }


        public IActionResult OnPost()
        {
            bool usernameExists;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string selectQuery = "SELECT COUNT(*) FROM Farmers WHERE username = @username";

                using (SqlCommand selectCmd = new SqlCommand(selectQuery, con))
                {
                    selectCmd.Parameters.AddWithValue("@username", username);
                    int count = (int)selectCmd.ExecuteScalar();
                    usernameExists = count > 0;
                }
            }

            if (usernameExists)
            {
                ErrorMessage = "Username " + username + " already exists. Please try again with a different username.";
                return Page();
            }
            else
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();

                    string query = "INSERT INTO farmers (username, password, roles, name, surname, category, contact) VALUES (@username, @password, @roles, @name, @surname, @category, @contact)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@surname", surname);
                        cmd.Parameters.AddWithValue("@category", category);
                        cmd.Parameters.AddWithValue("@contact", contact);
                        cmd.Parameters.AddWithValue("@roles", "Farmer");

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            return RedirectToPage("/EmployeeDashboard");
        }
    }
}
