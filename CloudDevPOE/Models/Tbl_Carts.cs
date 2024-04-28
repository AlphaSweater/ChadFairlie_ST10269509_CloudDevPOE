using CloudDevPOE.ViewModels;
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
		public decimal TotalValue { get; set; }

		//--------------------------------------------------------------------------------------------------------------------------//
		public bool IsActive { get; set; }

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public int GetActiveCart(int userId, string connectionString)
		{
			using (var con = new SqlConnection(connectionString))
			{
				con.Open();
				// Check if the user already has an active cart
				string cartCheckSql = "SELECT CartID FROM Tbl_Carts WHERE UserID = @UserID AND IsActive = 1";
				using (SqlCommand cartCheckCmd = new SqlCommand(cartCheckSql, con))
				{
					cartCheckCmd.Parameters.AddWithValue("@UserID", userId);
					int? cartId = (int)cartCheckCmd.ExecuteScalar();

					if (!cartId.HasValue)
					{
						// Create a new cart for the user
						string createCartSql = "INSERT INTO Tbl_Carts (UserID, IsActive) OUTPUT INSERTED.CartID VALUES (@UserID, 1)";
						using (SqlCommand createCartCmd = new SqlCommand(createCartSql, con))
						{
							createCartCmd.Parameters.AddWithValue("@UserID", userId);
							cartId = (int)createCartCmd.ExecuteScalar();
							return (int)cartId;
						}
					}
					return (int)cartId;
				}
			}
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public CartViewModel GetCartDetails(int userId, string connectionString)
		{
			CartViewModel cartViewModel = new CartViewModel();
			int cartId = GetActiveCart(userId, connectionString); // Use GetActiveCart to ensure an active cart exists and get its ID

			if (cartId > 0)
			{
				using (var con = new SqlConnection(connectionString))
				{
					con.Open();
					cartViewModel.CartID = cartId;
					cartViewModel.Items = new List<CartItemViewModel>();

					// Fetch each item's details and calculate each item's value
					string itemsSql = @"	SELECT ci.product_id, p.name, p.price, ci.quantity,
											(p.price * ci.quantity) AS Value
										FROM tbl_cart_items ci
										JOIN tbl_products p ON ci.product_id = p.product_id
										WHERE ci.cart_id = @CartID";

					using (SqlCommand itemsCmd = new SqlCommand(itemsSql, con))
					{
						itemsCmd.Parameters.AddWithValue("@CartID", cartId);
						using (var reader = itemsCmd.ExecuteReader())
						{
							while (reader.Read())
							{
								cartViewModel.Items.Add(new CartItemViewModel
								{
									ProductID = reader.GetInt32(0),
									ProductName = reader.GetString(1),
									Price = reader.GetDecimal(2),
									Quantity = reader.GetInt32(3),
									Value = reader.GetDecimal(4) // Individual item value
								});
							}
						}
					}

					// Calculate the total value of the cart
					string totalValueSql = @"SELECT SUM(p.price * ci.quantity) AS TotalValue
											FROM tbl_cart_items ci
											JOIN tbl_products p ON ci.product_id = p.product_id
											WHERE ci.cart_id = @CartID";

					using (SqlCommand totalValueCmd = new SqlCommand(totalValueSql, con))
					{
						totalValueCmd.Parameters.AddWithValue("@CartID", cartId);
						cartViewModel.TotalValue = (decimal)totalValueCmd.ExecuteScalar();
					}
				}
			}
			return cartViewModel;
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		// TODO: Implement Checkout Cart Method by calling getCartDetails and setting total_value to its proper value and add a transaction
		public void CheckoutCart(int cartId, string connectionString)
		{
			using (var con = new SqlConnection(connectionString))
			{
				con.Open();
				string sql = "UPDATE Tbl_Carts SET IsActive = 0 WHERE CartID = @CartID";
				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@CartID", cartId);
					cmd.ExecuteNonQuery();
				}
			}
		}
		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
	}
}