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
		public void RecordTransaction(int userId, CartViewModel cartDetails, string paymentMethod, SqlConnection con, SqlTransaction transaction)
		{
			string sql = @"INSERT INTO tbl_transactions (user_id, cart_id, total_value, transaction_date, payment_method)
                   VALUES (@UserId, @CartId, @TotalValue, @TransactionDate, @PaymentMethod)";
			using (SqlCommand cmd = new SqlCommand(sql, con, transaction))
			{
				cmd.Parameters.AddWithValue("@UserId", userId);
				cmd.Parameters.AddWithValue("@CartId", cartDetails.CartID);
				cmd.Parameters.AddWithValue("@TotalValue", cartDetails.TotalValue);
				cmd.Parameters.AddWithValue("@TransactionDate", DateTime.Now);
				cmd.Parameters.AddWithValue("@PaymentMethod", paymentMethod);
				cmd.ExecuteNonQuery();
			}
		}
	}
}