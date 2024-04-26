// Ignore Spelling: Tbl

using Microsoft.AspNetCore.Mvc;
using System.Data.Common;
using System.Data.SqlClient;
using System.Transactions;

namespace CloudDevPOE.Models
{
    public class Tbl_Product_Images
    {
        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
        public static string conString = "Server=tcp:st10269509-server.database.windows.net,1433;Initial Catalog=ST10269509-DB;Persist Security Info=False;User ID=AlphaSweater;Password=N@l@2004;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static SqlConnection con = new SqlConnection(conString);

        //--------------------------------------------------------------------------------------------------------------------------//
        // This only gets acquired after a Product is added to the database.
        public int? ProductID { get; set; }

        public string? ProductCategory { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public List<string>? ProductImageURLs { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public List<IFormFile>? ProductImages { get; set; }

        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//

        public List<string> SaveImagesToFileSystem(IWebHostEnvironment webHostEnvironment)
        {
            List<string> savedFilePaths = new List<string>();

            foreach (var file in this.ProductImages)
            {
                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var categoryPath = Path.Combine(webHostEnvironment.WebRootPath, "images", this.ProductCategory);
                Directory.CreateDirectory(categoryPath);
                var filePath = Path.Combine(categoryPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                savedFilePaths.Add($"/images/{this.ProductCategory}/{fileName}");
            }

            return savedFilePaths;
        }

        public void Insert_Images(List<IFormFile>? productImages, SqlConnection con, SqlTransaction transaction, IWebHostEnvironment webHostEnvironment)
        {
            this.ProductImages = productImages;

            List<string> filePaths = this.SaveImagesToFileSystem(webHostEnvironment);

            string sql = "INSERT INTO tbl_product_images (product_id, image_url) VALUES (@ProductID, @ProductImageURL)";
            SqlCommand cmd;

            foreach (var filePath in filePaths)
            {
                cmd = new SqlCommand(sql, con, transaction);
                cmd.Parameters.AddWithValue("@ProductID", this.ProductID);
                cmd.Parameters.AddWithValue("@ProductImageURL", filePath);
                cmd.ExecuteNonQuery();
            }
        }
    }
}