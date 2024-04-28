using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace CloudDevPOE.Models
{
	public class Tbl_Carts
	{
		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public int CartID { get; set; }

		//--------------------------------------------------------------------------------------------------------------------------//
		public int UserID { get; set; }

		//--------------------------------------------------------------------------------------------------------------------------//
		public bool IsActive { get; set; }

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public void Add_Item_ToCart(int userId, int productId, int quantity, string connectionString)
		{
			using (var con = new SqlConnection(connectionString))
			{
				con.Open();
				// Check if the user already has an active cart
				string cartCheckSql = "SELECT CartID FROM Tbl_Carts WHERE UserID = @UserID AND IsActive = 1";
				using (SqlCommand cartCheckCmd = new SqlCommand(cartCheckSql, con))
				{
					cartCheckCmd.Parameters.AddWithValue("@UserID", userId);
					var cartId = (int?)cartCheckCmd.ExecuteScalar();

					if (!cartId.HasValue)
					{
						// Create a new cart for the user
						string createCartSql = "INSERT INTO Tbl_Carts (UserID, IsActive) OUTPUT INSERTED.CartID VALUES (@UserID, 1)";
						using (SqlCommand createCartCmd = new SqlCommand(createCartSql, con))
						{
							createCartCmd.Parameters.AddWithValue("@UserID", userId);
							cartId = (int)createCartCmd.ExecuteScalar();
						}
					}

					// Add the product to the cart
					string addItemSql = "INSERT INTO Tbl_Cart_Items (CartID, ProductID, Quantity) VALUES (@CartID, @ProductID, @Quantity)";
					using (SqlCommand addItemCmd = new SqlCommand(addItemSql, con))
					{
						addItemCmd.Parameters.AddWithValue("@CartID", cartId.Value);
						addItemCmd.Parameters.AddWithValue("@ProductID", productId);
						addItemCmd.Parameters.AddWithValue("@Quantity", quantity);
						addItemCmd.ExecuteNonQuery();
					}
				}
			}
		}
	}
}