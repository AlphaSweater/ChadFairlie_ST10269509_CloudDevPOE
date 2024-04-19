// Ignore Spelling: Tbl

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace CloudDevPOE.Models
{
    public class Tbl_Users
    {
        public static string conString = "Server=tcp:st10269509-server.database.windows.net,1433;Initial Catalog=ST10269509-DB;Persist Security Info=False;User ID=AlphaSweater;Password=N@l@2004;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static SqlConnection con = new SqlConnection(conString);

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        [StringLength(50, ErrorMessage = "Surname cannot be longer than 50 characters.")]
        public string? Surname { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        [StringLength(255, ErrorMessage = "Email cannot be longer than 255 characters.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(128, ErrorMessage = "Password cannot be longer than 128 characters.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must be at least 8 characters and contain at least 1 uppercase letter, 1 lowercase letter, 1 digit, and 1 special character.")]
        public string? Password { get; set; }

        public int Insert_User(Tbl_Users m)
        {
            try
            {
                // Hash the password
                var passwordHasher = new PasswordHasher<IdentityUser>();
                var passwordHash = passwordHasher.HashPassword(user: null, password: m.Password);

                string sql = "INSERT INTO tbl_users (name, surname, email, password_hash) VALUES (@UserName, @UserSurname, @UserEmail, @PasswordHash)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@UserName", m.Name);
                cmd.Parameters.AddWithValue("@UserSurname", m.Surname);
                cmd.Parameters.AddWithValue("@UserEmail", m.Email);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();
                return rowsAffected;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For now, rethrow the exception
                throw ex;
            }
        }

        public bool Validate_User(Tbl_Users m)
        {
            try
            {
                string sql = "SELECT password_hash FROM tbl_users WHERE email = @UserEmail";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@UserEmail", m.Email);

                con.Open();
                string storedPasswordHash = cmd.ExecuteScalar() as string;
                con.Close();

                if (storedPasswordHash != null)
                {
                    var passwordHasher = new PasswordHasher<IdentityUser>();
                    var result = passwordHasher.VerifyHashedPassword(user: null, hashedPassword: storedPasswordHash, providedPassword: m.Password);
                    return result == PasswordVerificationResult.Success;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For now, rethrow the exception
                throw ex;
            }
        }
    }
}