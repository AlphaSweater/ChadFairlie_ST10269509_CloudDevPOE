using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace CloudDevPOE.Models
{
    public class Tbl_Cart_Items
    {
        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
        public static string conString = "Server=tcp:st10269509-server.database.windows.net,1433;Initial Catalog=ST10269509-DB;Persist Security Info=False;User ID=AlphaSweater;Password=N@l@2004;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static SqlConnection con = new SqlConnection(conString);

        //--------------------------------------------------------------------------------------------------------------------------//
        public int CartItemID { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public int CartID { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public int ProductID { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//
        public int Quantity { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------//

        //<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>//
    }
}