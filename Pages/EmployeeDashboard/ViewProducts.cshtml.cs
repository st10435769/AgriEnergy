using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using WebApplication3.Models;
using System.IO;
using System.Collections.Generic;

namespace WebApplication3.Pages.EmployeeDashboard
{
    public class ViewProductsModel : PageModel
    {
        public ViewProductsModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private readonly string _connectionString;

        public List<Product> Products { get; set; }

        public IActionResult OnPost()
        {
            string username = Request.Form["username"];

            if (string.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("username", "Username is required.");
                return Page();
            }

            Products = GetProductsByFarmerCategoryFromDatabase(username);

            if (Products.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "No products found for the specified farmer.");
            }

            return Page();
        }

        private List<Product> GetProductsByFarmerCategoryFromDatabase(string username)
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"SELECT p.ProductName, p.Category, p.ProductionDate, p.Quantity 
                                FROM Products p
                                WHERE p.Category IN (SELECT Category FROM Farmers WHERE Username = @username)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                ProductName = reader.GetString(0),
                                Category = reader.GetString(1),
                                ProductionDate = reader.GetDateTime(2),
                                Quantity = reader.GetInt32(3)
                            };
                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }

    }
    public class Product
    {
        public string ProductName { get; set; }
        public string Category { get; set; }
        public DateTime ProductionDate { get; set; }
        public int Quantity { get; set; }
    }
}
