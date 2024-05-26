using System.Data.SqlClient;

namespace CloudDevPOE.Models
{
	public class Tbl_Cart_Items
	{
		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public int CartItemID { get; set; }

		//--------------------------------------------------------------------------------------------------------------------------//
		public int CartID { get; set; }

		//--------------------------------------------------------------------------------------------------------------------------//
		public int ProductID { get; set; }

		//--------------------------------------------------------------------------------------------------------------------------//
		public int Quantity { get; set; }

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public void AddItemToCart(int cartId, int productId, int quantity, string connectionString)
		{
			using (var con = new SqlConnection(connectionString))
			{
				con.Open();

				// Check if the item already exists in the cart
				string checkItemSql = "SELECT cart_item_id, quantity FROM tbl_cart_items WHERE cart_id = @CartID AND product_id = @ProductID";
				using (SqlCommand checkItemCmd = new SqlCommand(checkItemSql, con))
				{
					checkItemCmd.Parameters.AddWithValue("@CartID", cartId);
					checkItemCmd.Parameters.AddWithValue("@ProductID", productId);
					bool itemExists = false;
					int cartItemId = 0;
					int currentQuantity = 0;
					using (var reader = checkItemCmd.ExecuteReader())
					{
						if (reader.Read())
						{
							// The item already exists in the cart, update the quantity
							cartItemId = reader.GetInt32(0);
							currentQuantity = reader.GetInt32(1);
							itemExists = true;
						}
					}

					if (itemExists)
					{
						string updateQuantitySql = "UPDATE tbl_cart_items SET quantity = @Quantity WHERE cart_item_id = @CartItemID";
						using (SqlCommand updateQuantityCmd = new SqlCommand(updateQuantitySql, con))
						{
							updateQuantityCmd.Parameters.AddWithValue("@CartItemID", cartItemId);
							updateQuantityCmd.Parameters.AddWithValue("@Quantity", currentQuantity + quantity);
							updateQuantityCmd.ExecuteNonQuery();
						}
					}
					else
					{
						// The item does not exist in the cart, add a new item
						string addItemSql = "INSERT INTO tbl_cart_items (cart_id, product_id, quantity) VALUES (@CartID, @ProductID, @Quantity)";
						using (SqlCommand addItemCmd = new SqlCommand(addItemSql, con))
						{
							addItemCmd.Parameters.AddWithValue("@CartID", cartId);
							addItemCmd.Parameters.AddWithValue("@ProductID", productId);
							addItemCmd.Parameters.AddWithValue("@Quantity", quantity);
							addItemCmd.ExecuteNonQuery();
						}
					}
				}
			}
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		// TODO: Implement Remove Item From Cart Method
		public void RemoveItemFromCart(int cartItemId, string connectionString)
		{
			using (var con = new SqlConnection(connectionString))
			{
				con.Open();
				string sql = "DELETE FROM tbl_cart_items WHERE cart_item_id = @CartItemID";
				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@CartItemID", cartItemId);
					cmd.ExecuteNonQuery();
				}
			}
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		// TODO: Implement Update Item Quantity Method
		public void UpdateItemQuantity(int cartItemId, int quantity, string connectionString)
		{
			using (var con = new SqlConnection(connectionString))
			{
				con.Open();
				string sql = "UPDATE tbl_cart_items SET quantity = @Quantity WHERE cart_item_id = @CartItemID";
				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@CartItemID", cartItemId);
					cmd.Parameters.AddWithValue("@Quantity", quantity);
					cmd.ExecuteNonQuery();
				}
			}
		}
	}
}