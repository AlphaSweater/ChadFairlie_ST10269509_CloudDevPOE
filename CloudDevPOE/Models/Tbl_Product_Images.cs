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

        // This only gets acquired after a Product is added to the database.
        public int ProductID { get; set; }

        public string ProductCategory { get; set; } = "Uncategorized";

        //--------------------------------------------------------------------------------------------------------------------------//
        public List<string>? ProductImageURLs { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public List<IFormFile>? ProductImages { get; set; }

        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//

        public List<string> SaveImagesToFileSystem(IWebHostEnvironment webHostEnvironment)
        {
            List<string> savedFilePaths = new List<string>();

            if (ProductImages == null || ProductImages.Count == 0)
            {
                // No images were uploaded, use the default image
                string defaultImagePath = "/images/Default/Default.jpg";
                savedFilePaths.Add(defaultImagePath);
                return savedFilePaths;
            }

            foreach (var file in ProductImages)
            {
                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var categoryPath = Path.Combine(webHostEnvironment.WebRootPath, "images", ProductCategory);
                Directory.CreateDirectory(categoryPath);
                var filePath = Path.Combine(categoryPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                savedFilePaths.Add($"/images/{ProductCategory}/{fileName}");
            }
            return savedFilePaths;
        }

        public void Insert_Images(SqlConnection con, SqlTransaction transaction, List<string> filePaths)
        {
            string sql = "INSERT INTO tbl_product_images (product_id, image_url) VALUES (@ProductID, @ProductImageURL)";
            foreach (var filePath in filePaths)
            {
                using (SqlCommand cmd = new SqlCommand(sql, con, transaction))
                {
                    cmd.Parameters.AddWithValue("@ProductID", ProductID);
                    cmd.Parameters.AddWithValue("@ProductImageURL", filePath);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}