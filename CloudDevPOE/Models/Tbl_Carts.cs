using CloudDevPOE.ViewModels;
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
		public int GetActiveCartID(int userId, string connectionString)
		{
			using (var con = new SqlConnection(connectionString))
			{
				con.Open();
				// Check if the user already has an active cart
				string cartCheckSql = "SELECT cart_id FROM tbl_carts WHERE user_id = @UserID AND is_active = 1";
				using (SqlCommand cartCheckCmd = new SqlCommand(cartCheckSql, con))
				{
					cartCheckCmd.Parameters.AddWithValue("@UserID", userId);
					object cartId = cartCheckCmd.ExecuteScalar();

					if (cartId == null)
					{
						// Create a new cart for the user
						string createCartSql = "INSERT INTO tbl_carts (user_id, is_active) OUTPUT INSERTED.cart_id VALUES (@UserID, 1)";
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

		//--------------------------------------------------------------------------------------------------------------------------//
		public CartViewModel GetCart(int cartId, string connectionString)
		{
			CartViewModel cartViewModel = new CartViewModel();

			if (cartId > 0)
			{
				using (var con = new SqlConnection(connectionString))
				{
					con.Open();
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
						using (var reader = itemsCmd.ExecuteReader())
						{
							while (reader.Read())
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
						object totalValueResult = totalValueCmd.ExecuteScalar();
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
					total = (decimal)cmd.ExecuteScalar();
				}
			}

			return total;
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public void CheckoutCart(int userId, string connectionString)
		{
			int activeCartId = GetActiveCartID(userId, connectionString);
			CartViewModel cartDetails = GetCart(activeCartId, connectionString);

			using (var con = new SqlConnection(connectionString))
			{
				con.Open();
				using (var transaction = con.BeginTransaction())
				{
					try
					{
						// Deactivate the cart
						string deactivateCartSql = "UPDATE tbl_carts SET is_active = 0 WHERE cart_id = @CartID";
						using (SqlCommand cmd = new SqlCommand(deactivateCartSql, con, transaction))
						{
							cmd.Parameters.AddWithValue("@CartID", cartDetails.CartID);
							cmd.ExecuteNonQuery();
						}

						// Reduce the quantity of each product in the cart
						foreach (var item in cartDetails.Items)
						{
							string reduceQuantitySql = "UPDATE tbl_products SET quantity = quantity - @Quantity WHERE product_id = @ProductID";
							using (SqlCommand cmd = new SqlCommand(reduceQuantitySql, con, transaction))
							{
								cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
								cmd.Parameters.AddWithValue("@ProductID", item.ProductID);
								cmd.ExecuteNonQuery();
							}
						}

						// Record the transaction using the RecordTransaction method
						Tbl_Transactions transactionModel = new Tbl_Transactions();
						transactionModel.RecordTransaction(userId, cartDetails, con, transaction);

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