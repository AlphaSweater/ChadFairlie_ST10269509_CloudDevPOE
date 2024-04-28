// Ignore Spelling: Tbl

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;

namespace CloudDevPOE.Models
{
    public class Tbl_Users
    {
        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
        public static string conString = "Server=tcp:st10269509-server.database.windows.net,1433;Initial Catalog=ST10269509-DB;Persist Security Info=False;User ID=AlphaSweater;Password=N@l@2004;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static SqlConnection con = new SqlConnection(conString);

        //--------------------------------------------------------------------------------------------------------------------------//
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
        public virtual int ExecuteNonQuery(SqlCommand cmd)
        {
            try
            {
                con.Open();
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
        }

        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
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

                return ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For now, rethrow the exception
                throw ex;
            }
        }

        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
        public virtual SqlDataReader ExecuteReader(SqlCommand cmd)
        {
            try
            {
                con.Open();
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                throw;
            }
        }

        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
        public bool Validate_User(Tbl_Users m, HttpContext httpContext)
        {
            try
            {
                string sql = "SELECT user_id, password_hash FROM tbl_users WHERE email = @UserEmail";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@UserEmail", m.Email);

                SqlDataReader reader = ExecuteReader(cmd);
                if (reader.Read())
                {
                    var passwordHasher = new PasswordHasher<IdentityUser>();
                    var result = passwordHasher.VerifyHashedPassword(user: null, hashedPassword: reader["password_hash"] as string, providedPassword: m.Password);
                    if (result == PasswordVerificationResult.Success)
                    {
                        m.UserID = int.Parse(reader["user_id"].ToString());
                        httpContext.Session.SetInt32("UserId", m.UserID);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For now, rethrow the exception
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
    }
}