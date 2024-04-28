using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace CloudDevPOE.Models
{
    public class Tbl_Carts
    {
        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
        public static string conString = "Server=tcp:st10269509-server.database.windows.net,1433;Initial Catalog=ST10269509-DB;Persist Security Info=False;User ID=AlphaSweater;Password=N@l@2004;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static SqlConnection con = new SqlConnection(conString);

        //--------------------------------------------------------------------------------------------------------------------------//
        public int CartID { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public int UserID { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public bool IsActive { get; set; }

        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
        public void Add_Item_ToCart(int userId, int productId, int quantity)
        {
            using (var con = new SqlConnection(conString))
            {
                con.Open();
                // Check if the user already has an active cart
                string cartCheckSql = "SELECT CartID FROM Tbl_Carts WHERE UserID = @UserID AND IsActive = 1";
                SqlCommand cartCheckCmd = new SqlCommand(cartCheckSql, con);
                cartCheckCmd.Parameters.AddWithValue("@UserID", userId);
                var cartId = (int?)cartCheckCmd.ExecuteScalar();

                if (!cartId.HasValue)
                {
                    // Create a new cart for the user
                    string createCartSql = "INSERT INTO Tbl_Carts (UserID, IsActive) OUTPUT INSERTED.CartID VALUES (@UserID, 1)";
                    SqlCommand createCartCmd = new SqlCommand(createCartSql, con);
                    createCartCmd.Parameters.AddWithValue("@UserID", userId);
                    cartId = (int)createCartCmd.ExecuteScalar();
                }

                // Add the product to the cart
                string addItemSql = "INSERT INTO Tbl_Cart_Items (CartID, ProductID, Quantity) VALUES (@CartID, @ProductID, @Quantity)";
                SqlCommand addItemCmd = new SqlCommand(addItemSql, con);
                addItemCmd.Parameters.AddWithValue("@CartID", cartId.Value);
                addItemCmd.Parameters.AddWithValue("@ProductID", productId);
                addItemCmd.Parameters.AddWithValue("@Quantity", quantity);
                addItemCmd.ExecuteNonQuery();
            }
        }
    }
}