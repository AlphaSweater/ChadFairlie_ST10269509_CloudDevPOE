// Ignore Spelling: Tbl

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;

namespace CloudDevPOE.Models
{
    public class Tbl_Products
    {
        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
        public static string conString = "Server=tcp:st10269509-server.database.windows.net,1433;Initial Catalog=ST10269509-DB;Persist Security Info=False;User ID=AlphaSweater;Password=N@l@2004;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static SqlConnection con = new SqlConnection(conString);

        //--------------------------------------------------------------------------------------------------------------------------//
        public int ProductID { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public int UserID { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public string ProductName { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public string ProductCategory { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public string ProductDescription { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public decimal ProductPrice { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public int ProductQuantity { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public bool ProductAvailability { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public string? ProductImageURL { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public List<IFormFile>? ProductImages { get; set; }

        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
        public virtual int ExecuteNonQuery(SqlCommand cmd)
        {
            try
            {
                con.Open();
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
        public int Insert_Product(Tbl_Products m, int userID, IWebHostEnvironment webHostEnvironment)
        {
            SqlTransaction transaction = null;
            try
            {
                con.Open();
                transaction = con.BeginTransaction(); // Start the transaction

                string sql = "INSERT INTO tbl_products (user_id, name, category, description, price, quantity, availability) OUTPUT INSERTED.product_id VALUES (@UserID, @ProductName, @ProductCategory, @ProductDescription, @ProductPrice, @ProductQuantity, @ProductAvailability)";
                SqlCommand cmd = new SqlCommand(sql, con, transaction); // Associate the command with the transaction
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@ProductName", m.ProductName);
                cmd.Parameters.AddWithValue("@ProductCategory", m.ProductCategory);
                cmd.Parameters.AddWithValue("@ProductDescription", m.ProductDescription);
                cmd.Parameters.AddWithValue("@ProductPrice", m.ProductPrice);
                cmd.Parameters.AddWithValue("@ProductQuantity", m.ProductQuantity);
                m.ProductAvailability = m.ProductQuantity > 0;
                cmd.Parameters.AddWithValue("@ProductAvailability", m.ProductAvailability);

                // Execute the command and retrieve the new ProductID
                m.ProductID = (int)cmd.ExecuteScalar();

                foreach (var file in m.ProductImages)
                {
                    // Extract the file extension
                    var fileExtension = Path.GetExtension(file.FileName);
                    // Generate a unique filename by appending a GUID
                    var fileName = $"{Guid.NewGuid()}{fileExtension}";
                    // Create a directory path that includes the product category
                    var categoryPath = Path.Combine(webHostEnvironment.WebRootPath, "images", m.ProductCategory);
                    // Ensure the directory exists
                    Directory.CreateDirectory(categoryPath);
                    // Combine the category path with the unique file name to get the full path
                    var filePath = Path.Combine(categoryPath, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    // Insert image URL into database, adjusting the path to include the product category
                    sql = "INSERT INTO tbl_product_images (product_id, image_url) VALUES (@ProductID, @ProductImageURL)";
                    cmd = new SqlCommand(sql, con, transaction);
                    cmd.Parameters.AddWithValue("@ProductID", m.ProductID);
                    // Adjust the URL to include the product category and the unique file name
                    cmd.Parameters.AddWithValue("@ProductImageURL", $"/images/{m.ProductCategory}/{fileName}");

                    cmd.ExecuteNonQuery();
                }

                transaction.Commit(); // Commit the transaction if all commands were successful
                return 1;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback(); // Roll back the transaction in case of an error
                }
                // Log the exception or handle it appropriately
                // For now, rethrow the exception
                throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
    }
}