// Ignore Spelling: Tbl

using CloudDevPOE.Services;
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
        public string ProductImageURL { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public IFormFile ProductImage { get; set; }

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
        public int Insert_Product(Tbl_Products m, int userID, ImageService imageService)
        {
            try
            {
                string sql = "INSERT INTO tbl_products (user_id, name, category, description, price, quantity, availability) VALUES (@UserID, @ProductName, @ProductCategory, @ProductDescription, @ProductPrice, @ProductQuantity, @ProductAvailability)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@ProductName", m.ProductName);
                cmd.Parameters.AddWithValue("@ProductCategory", m.ProductCategory);
                cmd.Parameters.AddWithValue("@ProductDescription", m.ProductDescription);
                cmd.Parameters.AddWithValue("@ProductPrice", m.ProductPrice);
                cmd.Parameters.AddWithValue("@ProductQuantity", m.ProductQuantity);
                cmd.Parameters.AddWithValue("@ProductAvailability", m.ProductAvailability);

                int result = ExecuteNonQuery(cmd);
                int result2 = 0;

                if (result == 1)
                {
                    // Upload the image and set the ProductImageURL
                    m.ProductImageURL = imageService.UploadImageAsync(m.ProductImage).Result;

                    sql = "INSERT INTO tbl_product_images (product_id, image_url) VALUES (@ProductID, @ProductImageURL)";
                    cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@ProductID", m.ProductID);
                    cmd.Parameters.AddWithValue("@ProductImageURL", m.ProductImageURL);

                    result2 = ExecuteNonQuery(cmd);
                }

                return result2;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For now, rethrow the exception
                throw ex;
            }
        }

        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
    }
}