// Ignore Spelling: Tbl

using CloudDevPOE.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;

namespace CloudDevPOE.Models
{
	public class Tbl_Users
	{
		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public int UserID { get; private set; }

		//--------------------------------------------------------------------------------------------------------------------------//
		[Required(ErrorMessage = "Name is required.")]
		[StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
		public string? Name { get; set; }

		//--------------------------------------------------------------------------------------------------------------------------//
		[Required(ErrorMessage = "Surname is required.")]
		[StringLength(50, ErrorMessage = "Surname cannot be longer than 50 characters.")]
		public string? Surname { get; set; }

		//--------------------------------------------------------------------------------------------------------------------------//
		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid Email Address.")]
		[StringLength(255, ErrorMessage = "Email cannot be longer than 255 characters.")]
		public string? Email { get; set; }

		//--------------------------------------------------------------------------------------------------------------------------//
		[Required(ErrorMessage = "Password is required.")]
		[StringLength(128, ErrorMessage = "Password cannot be longer than 128 characters.")]
		[DataType(DataType.Password)]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must be at least 8 characters and contain at least 1 uppercase letter, 1 lowercase letter, 1 digit, and 1 special character.")]
		public string? Password { get; set; }

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public int InsertUser(Tbl_Users m, string connectionString)
		{
			using (var con = new SqlConnection(connectionString))
			{
				con.Open();
				using (var transaction = con.BeginTransaction())
				{
					try
					{
						// Hash the password
						var passwordHasher = new PasswordHasher<IdentityUser>();
						var passwordHash = passwordHasher.HashPassword(user: null, password: m.Password);

						string sql = "INSERT INTO tbl_users (name, surname, email, password_hash) VALUES (@UserName, @UserSurname, @UserEmail, @PasswordHash)";
						using (SqlCommand cmd = new SqlCommand(sql, con, transaction))
						{
							cmd.Parameters.AddWithValue("@UserName", m.Name);
							cmd.Parameters.AddWithValue("@UserSurname", m.Surname);
							cmd.Parameters.AddWithValue("@UserEmail", m.Email);
							cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

							int result = cmd.ExecuteNonQuery();
							transaction.Commit(); // Commit the transaction
							return result;
						}
					}
					catch (Exception)
					{
						transaction.Rollback();
						throw;
					}
					finally
					{
						if (con.State == ConnectionState.Open)
							con.Close();
					}
				}
			}
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public int? ValidateUser(Tbl_Users m, string connectionString)
		{
			using (var con = new SqlConnection(connectionString))
			{
				con.Open();
				string sql = "SELECT user_id, password_hash FROM tbl_users WHERE email = @UserEmail";
				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@UserEmail", m.Email);
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							var passwordHasher = new PasswordHasher<IdentityUser>();
							var result = passwordHasher.VerifyHashedPassword(user: null, hashedPassword: reader["password_hash"] as string, providedPassword: m.Password);
							if (result == PasswordVerificationResult.Success)
							{
								return int.Parse(reader["user_id"].ToString());
							}
						}
						return null;
					}
				}
			}
		}

		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public async Task<UserAccountViewModel> GetUserDetailsAsync(int userID, string connectionString)
		{
			UserAccountViewModel user = new UserAccountViewModel();
			using (var con = new SqlConnection(connectionString))
			{
				await con.OpenAsync();
				string sql = "SELECT name, surname, email FROM tbl_users WHERE user_id = @UserID";
				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@UserID", userID);
					using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
					{
						if (await reader.ReadAsync())
						{
							user.Name = reader["name"].ToString();
							user.Surname = reader["surname"].ToString();
							user.Email = reader["email"].ToString();
						}
					}
				}
			}

			Tbl_Carts carts = new Tbl_Carts();
			Tbl_Products products = new Tbl_Products();
			Tbl_Transactions transactions = new Tbl_Transactions();
			int activeCartId = await carts.GetActiveCartIDAsync(userID, connectionString);
			user.ActiveCart = await carts.GetCartAsync(activeCartId, connectionString);
			user.PastOrders = await transactions.GetPastOrdersAsync(userID, connectionString);
			user.ListedProducts = await products.GetListedProductsAsync(userID, connectionString);

			return user;
		}


		//<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
		public NavbarViewModel GetNameAndCartCount(int userID, string connectionString)
		{
			NavbarViewModel userNavInfo = new NavbarViewModel();
			using (var con = new SqlConnection(connectionString))
			{
				con.Open();
				string sql = "SELECT name FROM tbl_users WHERE user_id = @UserID";
				using (SqlCommand cmd = new SqlCommand(sql, con))
				{
					cmd.Parameters.AddWithValue("@UserID", userID);
					using (SqlDataReader reader = cmd.ExecuteReader())
					{
						if (reader.Read())
						{
							userNavInfo.Name = reader["name"].ToString();
						}
					}
				}

				Tbl_Carts carts = new Tbl_Carts();
				userNavInfo.ActiveCartSize = carts.GetActiveCartItemCount(userID, connectionString);
			}

			return userNavInfo;
		}
	}
}