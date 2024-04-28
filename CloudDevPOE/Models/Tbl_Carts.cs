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

		// TODO: Implement Get Cart Items Method
		//public CartDetails GetActiveCartDetails(int userId, string connectionString)
		//{
		//	// Logic to retrieve active cart details, including items and total value
		//}
	}
}