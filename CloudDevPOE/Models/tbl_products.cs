// Ignore Spelling: Tbl

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace CloudDevPOE.Models
{
    public class tbl_products
    {
        public static string conString = "Server=tcp:st10269509-server.database.windows.net,1433;Initial Catalog=ST10269509-DB;Persist Security Info=False;User ID=AlphaSweater;Password=N@l@2004;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static SqlConnection con = new SqlConnection(conString);

        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductCategory { get; set; }
        public bool ProductAvailability { get; set; }

        public int Insert_Product(tbl_products m)
        {
            try
            {
                string sql = "INSERT INTO tbl_products (user_id, name, category, description, price, quantity, availability) VALUES (@ProductName, @ProductCategory, @ProductPrice, @ProductAvailability)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@ProductName", m.ProductName);
                cmd.Parameters.AddWithValue("@ProductPrice", m.ProductPrice);
                cmd.Parameters.AddWithValue("@ProductCategory", m.ProductCategory);
                cmd.Parameters.AddWithValue("@ProductAvailability", m.ProductAvailability);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();
                return rowsAffected;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For now, rethrow the exception
                throw ex;
            }
        }
    }
}