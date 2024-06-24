﻿// Ignore Spelling: Tbl

using CloudDevPOE.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;

namespace CloudDevPOE.Models
{
	public class Tbl_Products
	{
		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
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
		public bool IsArchived { get; set; }

		//--------------------------------------------------------------------------------------------------------------------------//
		public Tbl_Product_Images ProductImagesModel { get; set; } = new Tbl_Product_Images();

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public int InsertProduct(Tbl_Products m, int userID, IWebHostEnvironment webHostEnvironment, string connectionString)
		{
			using (var con = new SqlConnection(connectionString))
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

						m.ProductImagesModel.InsertImages(con, transaction, savedImagePaths);

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
			}
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public List<ProductSummaryViewModel> ListProducts(string connectionString)
		{
			List<ProductSummaryViewModel> productSummaries = new List<ProductSummaryViewModel>();

			using (var con = new SqlConnection(connectionString))
			{
				con.Open();
				string productSql =
				@"  SELECT
            tp.product_id,
            tp.name,
            tp.category,
            tp.description,
            tp.price,
            tp.availability,
            tpi.image_url
        FROM
            ((tbl_products tp
        INNER JOIN
            (
            SELECT MIN(image_id) AS image_id, product_id
            FROM tbl_product_images
            GROUP BY product_id
            ) AS first_image ON tp.product_id = first_image.product_id)
        INNER JOIN
            tbl_product_images tpi ON first_image.image_id = tpi.image_id)
        WHERE tp.is_archived = 0 AND tp.quantity > 0"; // Exclude archived products and products with quantity less than 1

				using (var productCmd = new SqlCommand(productSql, con))
				{
					using (var reader = productCmd.ExecuteReader())
					{
						while (reader.Read())
						{
							productSummaries.Add(new ProductSummaryViewModel
							{
								ProductID = (int)reader["product_id"],
								ProductName = reader["name"].ToString(),
								ProductCategory = reader["category"].ToString(),
								ProductDescription = reader["description"].ToString(),
								ProductPrice = (decimal)reader["price"],
								ProductAvailability = (bool)reader["availability"],
								ProductMainImageUrl = reader["image_url"].ToString()
							});
						}
					}
				}
			}
			return productSummaries;
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public ProductSummaryViewModel GetProductSummaryById(int productId, string connectionString)
		{
			ProductSummaryViewModel productSummary = null;

			using (var con = new SqlConnection(connectionString))
			{
				con.Open();
				string productSql =
				@"SELECT
            tp.product_id,
            tp.name,
            tp.category,
            tp.description,
            tp.price,
            tp.availability,
            tpi.image_url
        FROM
            tbl_products tp
        INNER JOIN
            tbl_product_images tpi ON tp.product_id = tpi.product_id
        WHERE tp.product_id = @ProductId AND tp.is_archived = 0";

				using (var productCmd = new SqlCommand(productSql, con))
				{
					productCmd.Parameters.AddWithValue("@ProductId", productId);

					using (var reader = productCmd.ExecuteReader())
					{
						if (reader.Read())
						{
							productSummary = new ProductSummaryViewModel
							{
								ProductID = (int)reader["product_id"],
								ProductName = reader["name"].ToString(),
								ProductCategory = reader["category"].ToString(),
								ProductDescription = reader["description"].ToString(),
								ProductPrice = (decimal)reader["price"],
								ProductAvailability = (bool)reader["availability"],
								ProductMainImageUrl = reader["image_url"].ToString()
							};
						}
					}
				}
			}
			return productSummary;
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public ProductDetailsViewModel ViewProduct(int productID, string connectionString)
		{
			ProductDetailsViewModel productDetails = null;

			using (var con = new SqlConnection(connectionString))
			{
				con.Open();
				// Fetch the product details
				string productSql = @"SELECT tblp.*, tblu.name AS seller_name
                              FROM tbl_products tblp
                              INNER JOIN tbl_users tblu ON tblp.user_id = tblu.user_id
                              WHERE tblp.product_id = @ProductID";
				using (var productCmd = new SqlCommand(productSql, con))
				{
					productCmd.Parameters.AddWithValue("@ProductID", productID);
					using (var reader = productCmd.ExecuteReader())
					{
						if (reader.Read())
						{
							productDetails = new ProductDetailsViewModel
							{
								ProductID = productID,
								SellerUserID = (int)reader["user_id"],
								SellerName = reader["seller_name"].ToString(),
								ProductName = reader["name"].ToString(),
								ProductCategory = reader["category"].ToString(),
								ProductDescription = reader["description"].ToString(),
								AvailableQuantity = (int)reader["quantity"],
								ProductPrice = (decimal)reader["price"],
								ImageUrls = new List<string>() // Initialize the list to be filled
							};
						}
					}
				}

				// Assuming the product was found, fetch its images
				if (productDetails != null)
				{
					string imagesSql = "SELECT image_url FROM tbl_product_images WHERE product_id = @ProductID";
					using (var imagesCmd = new SqlCommand(imagesSql, con))
					{
						imagesCmd.Parameters.AddWithValue("@ProductID", productID);
						using (var reader = imagesCmd.ExecuteReader())
						{
							while (reader.Read())
							{
								productDetails.ImageUrls.Add(reader["image_url"].ToString());
							}
						}
					}
				}
			}
			return productDetails;
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public async Task<List<ProductDetailsViewModel>> GetListedProductsAsync(int userID, string connectionString)
		{
			List<ProductDetailsViewModel> products = new List<ProductDetailsViewModel>();

			using (var con = new SqlConnection(connectionString))
			{
				await con.OpenAsync();
				string productSql = @"SELECT tblp.*, tblu.name AS seller_name, tpi.image_url AS main_image_url
              FROM tbl_products tblp
              INNER JOIN tbl_users tblu ON tblp.user_id = tblu.user_id
              LEFT JOIN (
                  SELECT MIN(image_id) AS image_id, product_id
                  FROM tbl_product_images
                  GROUP BY product_id
              ) AS first_image ON tblp.product_id = first_image.product_id
              LEFT JOIN tbl_product_images tpi ON first_image.image_id = tpi.image_id
              WHERE tblp.user_id = @UserID AND tblp.is_archived = 0";
				using (var productCmd = new SqlCommand(productSql, con))
				{
					productCmd.Parameters.AddWithValue("@UserID", userID);
					using (var reader = await productCmd.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync())
						{
							var product = new ProductDetailsViewModel
							{
								ProductID = (int)reader["product_id"],
								SellerUserID = (int)reader["user_id"],
								SellerName = reader["seller_name"].ToString(),
								ProductName = reader["name"].ToString(),
								ProductCategory = reader["category"].ToString(),
								ProductDescription = reader["description"].ToString(),
								AvailableQuantity = (int)reader["quantity"],
								ProductPrice = (decimal)reader["price"],
								ProductMainImageUrl = reader["main_image_url"] as string // Set the main image URL
							};

							products.Add(product);
						}
					}
				}
			}

			return products;
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public async Task AddQuantityAsync(int productId, int quantityToAdd, string connectionString)
		{
			using (var con = new SqlConnection(connectionString))
			{
				await con.OpenAsync();

				string sql = "UPDATE tbl_products SET quantity = quantity + @QuantityToAdd WHERE product_id = @ProductId";

				using (var cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@ProductId", productId);
					cmd.Parameters.AddWithValue("@QuantityToAdd", quantityToAdd);

					await cmd.ExecuteNonQueryAsync();
				}
			}
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public async Task ArchiveProductAsync(int productId, string connectionString)
		{
			using (var con = new SqlConnection(connectionString))
			{
				await con.OpenAsync();

				string sql = "UPDATE tbl_products SET is_archived = 1 WHERE product_id = @ProductId";

				using (var cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@ProductId", productId);

					await cmd.ExecuteNonQueryAsync();
				}
			}
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
	}
}