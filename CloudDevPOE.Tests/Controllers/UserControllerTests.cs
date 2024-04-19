using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CloudDevPOE.Models;

namespace CloudDevPOE.Tests.Controllers
{
    public class MockUserRepository
    {
        public bool Validate_User(Tbl_Users user)
        {
            return user.Email == "test@example.com" && user.Password == "TestPassword123!";
        }

        public int Insert_User(Tbl_Users user)
        {
            // Implement this if needed
            throw new NotImplementedException();
        }
    }

    public class UserControllerTests
    {
        [Fact]
        public void Validate_User_WithMockValidCredentials_ReturnsTrue()
        {
            // Arrange
            var user = new Tbl_Users
            {
                Email = "test@example.com",
                Password = "TestPassword123!"
            };
            var userRepository = new MockUserRepository();

            // Act
            bool isValidUser = userRepository.Validate_User(user);

            // Assert
            Assert.True(isValidUser);
        }

        [Fact]
        public void Validate_User_WithRealValidCredentials_ReturnsTrue()
        {
            // Arrange
            var user = new Tbl_Users
            {
                Email = "admin@gmail.com",
                Password = "admin"
            };

            // Act
            bool isValidUser = user.Validate_User(user);

            // Assert
            Assert.True(isValidUser);
        }
    }
}