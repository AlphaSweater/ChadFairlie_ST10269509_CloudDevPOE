using CloudDevPOE.ViewModels;
using System.Data.SqlClient;

namespace CloudDevPOE.Models
{
	public class Tbl_Transactions
	{
		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public int TransactionId { get; set; }

		//--------------------------------------------------------------------------------------------------------------------------//
		public int UserId { get; set; }

		//--------------------------------------------------------------------------------------------------------------------------//
		public int CartId { get; set; }

		//--------------------------------------------------------------------------------------------------------------------------//
		public decimal TotalValue { get; set; }

		//--------------------------------------------------------------------------------------------------------------------------//
		public DateTime TransactionDate { get; set; }

		//--------------------------------------------------------------------------------------------------------------------------//
		public string PaymentMethod { get; set; }

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public void RecordTransaction(int userId, CartViewModel cartDetails, SqlConnection con, SqlTransaction transaction)
		{
			string sql = @"INSERT INTO tbl_transactions (user_id, cart_id, total_value, transaction_date)
                   VALUES (@UserId, @CartId, @TotalValue, @TransactionDate)";
			using (SqlCommand cmd = new SqlCommand(sql, con, transaction))
			{
				cmd.Parameters.AddWithValue("@UserId", userId);
				cmd.Parameters.AddWithValue("@CartId", cartDetails.CartID);
				cmd.Parameters.AddWithValue("@TotalValue", cartDetails.TotalValue);
				cmd.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
				cmd.ExecuteNonQuery();
			}
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public List<PastCartViewModel> GetPastOrders(int userId, string connectionString)
		{
			List<PastCartViewModel> pastOrders = new List<PastCartViewModel>();

			using (var con = new SqlConnection(connectionString))
			{
				con.Open();

				// Fetch each past order's details
				string ordersSql = @"	SELECT t.cart_id, t.total_value, t.transaction_date
										FROM tbl_transactions t
										JOIN tbl_carts c ON t.cart_id = c.cart_id
										WHERE c.user_id = @UserID AND c.is_active = 0";

				using (SqlCommand ordersCmd = new SqlCommand(ordersSql, con))
				{
					ordersCmd.Parameters.AddWithValue("@UserID", userId);
					using (var reader = ordersCmd.ExecuteReader())
					{
						while (reader.Read())
						{
							int CartId = reader.GetInt32(0);
							pastOrders.Add(new PastCartViewModel
							{
								Cart = new Tbl_Carts().GetCart(CartId, connectionString),
								TotalValue = reader.GetDecimal(1),
								TransactionDate = reader.GetDateTime(2)
							});
						}
					}
				}
			}

			return pastOrders;
		}
	}
}