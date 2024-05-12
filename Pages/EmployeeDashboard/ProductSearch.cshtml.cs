using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace WebApplication3.Pages.EmployeeDashboard
{
    public class ProductSearchModel : PageModel
    {

        public ProductSearchModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private readonly string _connectionString;

        public List<Product> Products { get; set; }

        public IActionResult OnGet(string category, DateTime? startDate, DateTime? endDate)
        {
            Products = GetProductsFromDatabase(category, startDate, endDate);
            return Page();
        }

        private List<Product> GetProductsFromDatabase(string category, DateTime? startDate, DateTime? endDate)
        {
            List<Product> products = new List<Product>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query = "SELECT ProductName, Category, ProductionDate, Quantity FROM Products WHERE 1=1";

                if (!string.IsNullOrEmpty(category))
                {
                    query += " AND Category LIKE '%' + @category + '%'";
                }

                if (startDate != null)
                {
                    query += " AND ProductionDate >= @startDate";
                }

                if (endDate != null)
                {
                    query += " AND ProductionDate <= @endDate";
                }

                using (SqlCommand command = new SqlCommand(query, con))
                {
                    if (!string.IsNullOrEmpty(category))
                    {
                        command.Parameters.AddWithValue("@category", category);
                    }

                    if (startDate != null)
                    {
                        command.Parameters.AddWithValue("@startDate", startDate);
                    }

                    if (endDate != null)
                    {
                        command.Parameters.AddWithValue("@endDate", endDate);
                    }

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

}
