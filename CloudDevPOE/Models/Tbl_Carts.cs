using CloudDevPOE.ViewModels;
using System.Data;
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
		public async Task<int> GetActiveCartIDAsync(int userId, string connectionString)
		{
			using (var con = new SqlConnection(connectionString))
			{
				await con.OpenAsync();
				// Check if the user already has an active cart
				string cartCheckSql = "SELECT cart_id FROM tbl_carts WHERE user_id = @UserID AND is_active = 1";
				using (SqlCommand cartCheckCmd = new SqlCommand(cartCheckSql, con))
				{
					cartCheckCmd.Parameters.AddWithValue("@UserID", userId);
					object cartId = await cartCheckCmd.ExecuteScalarAsync();

					if (cartId == null)
					{
						// Create a new cart for the user
						string createCartSql = "INSERT INTO tbl_carts (user_id, is_active) OUTPUT INSERTED.cart_id VALUES (@UserID, 1)";
						using (SqlCommand createCartCmd = new SqlCommand(createCartSql, con))
						{
							createCartCmd.Parameters.AddWithValue("@UserID", userId);
							cartId = await createCartCmd.ExecuteScalarAsync();
							return (int)cartId;
						}
					}
					return (int)cartId;
				}
			}
		}

		//--------------------------------------------------------------------------------------------------------------------------//
		public async Task<CartViewModel> GetCartAsync(int cartId, string connectionString)
		{
			CartViewModel cartViewModel = new CartViewModel();

			if (cartId > 0)
			{
				using (var con = new SqlConnection(connectionString))
				{
					await con.OpenAsync();
					cartViewModel.CartID = cartId;
					cartViewModel.Items = new List<CartItemViewModel>();

					// Fetch each item's details, image, available quantity and calculate each item's value
					string itemsSql = @"	SELECT ci.cart_item_id, ci.product_id, p.name, p.price, ci.quantity,
                                        (p.price * ci.quantity) AS Value, pi.image_url, p.quantity
                                    FROM tbl_cart_items ci
                                    JOIN tbl_products p ON ci.product_id = p.product_id
                                    LEFT JOIN (
                                        SELECT product_id, MIN(image_url) as image_url
                                        FROM tbl_product_images
                                        GROUP BY product_id
                                    ) pi ON p.product_id = pi.product_id
                                    WHERE ci.cart_id = @CartID";

					using (SqlCommand itemsCmd = new SqlCommand(itemsSql, con))
					{
						itemsCmd.Parameters.AddWithValue("@CartID", cartId);
						using (var reader = await itemsCmd.ExecuteReaderAsync())
						{
							while (await reader.ReadAsync())
							{
								var item = new CartItemViewModel
								{
									CartItemID = reader.GetInt32(0),
									ProductID = reader.GetInt32(1),
									ProductName = reader.GetString(2),
									Price = reader.GetDecimal(3),
									Quantity = reader.GetInt32(4),
									Value = reader.GetDecimal(5),
									ImageUrl = reader.IsDBNull(6) ? "/images/Default/DefaultIcon.jpg" : reader.GetString(6),
									AvailableQuantity = reader.GetInt32(7)
								};

								cartViewModel.Items.Add(item);
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
						object totalValueResult = await totalValueCmd.ExecuteScalarAsync();
						cartViewModel.TotalValue = totalValueResult == DBNull.Value ? 0 : (decimal)totalValueResult;
					}
				}
			}
			return cartViewModel;
		}

		//--------------------------------------------------------------------------------------------------------------------------//
		public int GetActiveCartItemCount(int userID, string connectionString)
		{
			int itemCount = 0;
			using (var con = new SqlConnection(connectionString))
			{
				con.Open();
				string sql = @"SELECT COUNT(*)
                       FROM tbl_cart_items ci
                       JOIN tbl_carts c ON ci.cart_id = c.cart_id
                       WHERE c.user_id = @UserID AND c.is_active = 1";
				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@UserID", userID);
					itemCount = (int)cmd.ExecuteScalar();
				}
			}

			return itemCount;
		}

		//--------------------------------------------------------------------------------------------------------------------------//
		public decimal GetCartTotal(int cartId, string connectionString)
		{
			decimal total = 0;
			using (var con = new SqlConnection(connectionString))
			{
				con.Open();
				string sql = @"SELECT SUM(p.price * ci.quantity) AS TotalValue
                FROM tbl_cart_items ci
                JOIN tbl_products p ON ci.product_id = p.product_id
                WHERE ci.cart_id = @CartID";
				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@CartID", cartId);
					object result = cmd.ExecuteScalar();
					total = result == DBNull.Value ? 0 : (decimal)result;
				}
			}

			return total;
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public async Task CheckoutCartAsync(int userId, string connectionString)
		{
			int activeCartId = await GetActiveCartIDAsync(userId, connectionString);
			CartViewModel cartDetails = await GetCartAsync(activeCartId, connectionString);

			using (var con = new SqlConnection(connectionString))
			{
				await con.OpenAsync();
				using (var transaction = con.BeginTransaction())
				{
					try
					{
						// Create a DataTable to hold the cart items
						DataTable cartItemsTable = new DataTable();
						cartItemsTable.Columns.Add("ProductID", typeof(int));
						cartItemsTable.Columns.Add("Quantity", typeof(int));

						// Add the cart items to the DataTable
						foreach (var item in cartDetails.Items)
						{
							cartItemsTable.Rows.Add(item.ProductID, item.Quantity);
						}

						// Create a SqlParameter for the table-valued parameter
						SqlParameter cartItemsParameter = new SqlParameter("@CartItems", SqlDbType.Structured);
						cartItemsParameter.TypeName = "dbo.CartItemType";
						cartItemsParameter.Value = cartItemsTable;

						// Execute the stored procedure
						using (SqlCommand cmd = new SqlCommand("dbo.CheckoutCart", con, transaction))
						{
							cmd.CommandType = CommandType.StoredProcedure;
							cmd.Parameters.AddWithValue("@CartID", cartDetails.CartID);
							cmd.Parameters.Add(cartItemsParameter);
							await cmd.ExecuteNonQueryAsync();
						}

						// Record the transaction using the RecordTransaction method
						Tbl_Transactions transactionModel = new Tbl_Transactions();
						await transactionModel.RecordTransactionAsync(userId, cartDetails, con, transaction);

						// Commit the transaction
						transaction.Commit();
					}
					catch
					{
						// Rollback the transaction in case of an error
						transaction.Rollback();
						throw;
					}
				}
			}
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
	}
}