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
		public Tbl_Product_Images ProductImagesModel { get; set; } = new Tbl_Product_Images();

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

				m.ProductImagesModel.ProductID = m.ProductID; // Set the ProductID for image records
				m.ProductImagesModel.ProductCategory = m.ProductCategory; // Set the ProductCategory for image records

				transaction = m.ProductImagesModel.Insert_Images(con, transaction, webHostEnvironment);

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