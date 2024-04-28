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
        private readonly string _conString;

        public Tbl_Products(IConfiguration configuration)
        {
            _conString = configuration.GetConnectionString("DefaultConnection");
        }

        //--------------------------------------------------------------------------------------------------------------------------//
        public int ProductID { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public int UserID { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        [Required]
        public string ProductName { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        [Required]
        public string ProductCategory { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        [Required]
        public string ProductDescription { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        [Required]
        public decimal ProductPrice { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        [Required]
        public int ProductQuantity { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public bool ProductAvailability { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public Tbl_Product_Images ProductImagesModel { get; set; } = new Tbl_Product_Images();

        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
        public int Insert_Product(Tbl_Products m, int userID, IWebHostEnvironment webHostEnvironment)
        {
            using (var con = new SqlConnection(_conString))
            {
                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
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

                        m.ProductImagesModel.ProductID = m.ProductID; // Set the ProductID for image records
                        m.ProductImagesModel.ProductCategory = m.ProductCategory; // Set the ProductCategory for image records

                        // Save images to the file system and get their paths
                        List<string> savedImagePaths = m.ProductImagesModel.SaveImagesToFileSystem(webHostEnvironment);

                        m.ProductImagesModel.Insert_Images(con, transaction, savedImagePaths);

                        transaction.Commit(); // Commit the transaction if all commands were successful
                        return 1;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
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
    }
}