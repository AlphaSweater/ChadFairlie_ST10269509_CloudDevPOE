using CloudDevPOE.Models;
using Microsoft.AspNetCore.Identity;
using System.Data.SqlClient;

namespace CloudDevPOE.Tests.Models
{
    public class Tbl_UsersTests
    {
        private class TestableTbl_Users : Tbl_Users
        {
            public SqlCommand LastCommand { get; private set; }

            public override int ExecuteNonQuery(SqlCommand cmd)
            {
                // Instead of executing the command, just save it and return a dummy value
                LastCommand = cmd;
                return 1;
            }
        }

        [Fact]
        public void Insert_User_Calls_ExecuteNonQuery_With_Correct_Command()
        {
            // Arrange
            var user = new Tbl_Users
            {
                Name = "Test",
                Surname = "User",
                Email = "test@example.com",
                Password = "Password123!"
            };
            var testableTblUsers = new TestableTbl_Users();

            // Act
            testableTblUsers.Insert_User(user);

            // Assert
            Assert.NotNull(testableTblUsers.LastCommand);
            Assert.Equal("INSERT INTO tbl_users (name, surname, email, password_hash) VALUES (@UserName, @UserSurname, @UserEmail, @PasswordHash)", testableTblUsers.LastCommand.CommandText);
            Assert.Equal(user.Name, testableTblUsers.LastCommand.Parameters["@UserName"].Value);
            Assert.Equal(user.Surname, testableTblUsers.LastCommand.Parameters["@UserSurname"].Value);
            Assert.Equal(user.Email, testableTblUsers.LastCommand.Parameters["@UserEmail"].Value);

            // Verify that the password is hashed
            var passwordHasher = new PasswordHasher<IdentityUser>();
            var actualPasswordHash = testableTblUsers.LastCommand.Parameters["@PasswordHash"].Value.ToString();
            var verificationResult = passwordHasher.VerifyHashedPassword(user: null, hashedPassword: actualPasswordHash, providedPassword: user.Password);
            Assert.Equal(PasswordVerificationResult.Success, verificationResult);
        }
    }
}