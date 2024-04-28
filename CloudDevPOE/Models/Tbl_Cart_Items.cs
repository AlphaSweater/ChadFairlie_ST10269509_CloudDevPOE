using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

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



		public void Add_Item_To_Cart(int cartId, int productId, int quantity, string connectionString)
		{
			using (var con = new SqlConnection(connectionString))
			{
				con.Open();
				string sql = "INSERT INTO Tbl_Cart_Items (CartID, ProductID, Quantity) VALUES (@CartID, @ProductID, @Quantity)";
				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@CartID", cartId);
					cmd.Parameters.AddWithValue("@ProductID", productId);
					cmd.Parameters.AddWithValue("@Quantity", quantity);
					cmd.ExecuteNonQuery();
				}
			}
		}

		// TODO: Implement Remove Item From Cart Method
		public void RemoveItemFromCart(int cartItemId, string connectionString)
		{
			// Logic to remove an item from the cart
		}

		// TODO: Implement Update Item Quantity Method
		public void UpdateItemQuantity(int cartItemId, int quantity, string connectionString)
		{
			// Logic to update the quantity of an existing item in the cart
		}

	}
}