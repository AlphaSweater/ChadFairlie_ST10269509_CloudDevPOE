using CloudDevPOE.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDevPOE.Tests.Models
{
    public class Tbl_ProductsTests
    {
        private class TestableTbl_Products : Tbl_Products
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
        public void Insert_Product_Calls_ExecuteNonQuery_With_Correct_Command()
        {
            // Arrange
            var product = new Tbl_Products
            {
                ProductName = "Test Product",
                ProductCategory = "Test Category",
                ProductDescription = "Test Description",
                ProductPrice = 100,
                ProductQuantity = 10,
                ProductAvailability = true
            };
            var testableTblProducts = new TestableTbl_Products();
            int testUserId = 1;

            // Act
            testableTblProducts.Insert_Product(product, testUserId);

            // Assert
            Assert.NotNull(testableTblProducts.LastCommand);
            Assert.Equal("INSERT INTO tbl_products (user_id, name, category, description, price, quantity, availability) VALUES (@UserID, @ProductName, @ProductCategory, @ProductDescription, @ProductPrice, @ProductQuantity, @ProductAvailability)", testableTblProducts.LastCommand.CommandText);
            Assert.Equal(testUserId, testableTblProducts.LastCommand.Parameters["@UserID"].Value);
            Assert.Equal(product.ProductName, testableTblProducts.LastCommand.Parameters["@ProductName"].Value);
            Assert.Equal(product.ProductCategory, testableTblProducts.LastCommand.Parameters["@ProductCategory"].Value);
            Assert.Equal(product.ProductDescription, testableTblProducts.LastCommand.Parameters["@ProductDescription"].Value);
            Assert.Equal(product.ProductPrice, testableTblProducts.LastCommand.Parameters["@ProductPrice"].Value);
            Assert.Equal(product.ProductQuantity, testableTblProducts.LastCommand.Parameters["@ProductQuantity"].Value);
            Assert.Equal(product.ProductAvailability, testableTblProducts.LastCommand.Parameters["@ProductAvailability"].Value);
        }
    }
}