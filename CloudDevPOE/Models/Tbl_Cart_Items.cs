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
		// TODO: Implement Add Item To Cart Method
		public void AddItemToCart(int cartId, int productId, int quantity, string connectionString)
		{
			using (var con = new SqlConnection(connectionString))
			{
				con.Open();
				string sql = "INSERT INTO tbl_cart_items (cart_id, product_id, quantity) VALUES (@CartID, @ProductID, @Quantity)";
				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@CartID", cartId);
					cmd.Parameters.AddWithValue("@ProductID", productId);
					cmd.Parameters.AddWithValue("@Quantity", quantity);
					cmd.ExecuteNonQuery();
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
				string sql = "DELETE FROM Tbl_Cart_Items WHERE CartItemID = @CartItemID";
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
				string sql = "UPDATE Tbl_Cart_Items SET Quantity = @Quantity WHERE CartItemID = @CartItemID";
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