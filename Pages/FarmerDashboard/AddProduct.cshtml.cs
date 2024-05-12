using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;

namespace WebApplication3.Pages.FarmerDashboard
{
    public class AddProductModel : PageModel
    {
        private readonly string _connectionString;

        [BindProperty]
        public string ProductName { get; set; }

        [BindProperty]
        public string Category { get; set; }

        [BindProperty]
        public int Quantity { get; set; }

        public AddProductModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult OnPost()
        {
            // Generate a random product ID
            Random random = new Random();
            int productId = random.Next(100000, 999999); // Adjust the range as needed

            // Check if the product name exists
            bool productNameExists;
            int existingQuantity = 0;

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string selectQuery = "SELECT COUNT(*) FROM Products WHERE ProductName = @productName";
                using (SqlCommand selectCmd = new SqlCommand(selectQuery, con))
                {
                    selectCmd.Parameters.AddWithValue("@productName", ProductName);
                    int count = (int)selectCmd.ExecuteScalar();
                    productNameExists = count > 0;

                    // If the product name exists, get the existing quantity
                    if (productNameExists)
                    {
                        string quantityQuery = "SELECT Quantity FROM Products WHERE ProductName = @productName";
                        using (SqlCommand quantityCmd = new SqlCommand(quantityQuery, con))
                        {
                            quantityCmd.Parameters.AddWithValue("@productName", ProductName);
                            existingQuantity = (int)quantityCmd.ExecuteScalar();
                        }
                    }
                }
            }

            // Calculate the new quantity
            int newQuantity = existingQuantity + Quantity;

            // Insert or update the product
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                string query;
                if (productNameExists)
                {
                    // Update the existing product
                    query = "UPDATE Products SET Quantity = @newQuantity, ProductionDate = GETDATE() WHERE ProductName = @productName";
                }
                else
                {
                    // Insert a new product
                    query = "INSERT INTO Products (ProductId, ProductName, Category, ProductionDate, Quantity) VALUES (@productId, @productName, @category, GETDATE(), @newQuantity)";
                }

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@productId", productId);
                    cmd.Parameters.AddWithValue("@productName", ProductName);
                    cmd.Parameters.AddWithValue("@category", Category);
                    cmd.Parameters.AddWithValue("@newQuantity", newQuantity);

                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/FarmerDashboard");
        }
    }
}
