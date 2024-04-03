using System.Data.SqlClient;

namespace CloudDevPOE.Models
{
    public class tblUsers
    {
        public static string conString = "Server=tcp:st10269509-server.database.windows.net,1433;Initial Catalog=ST10269509-DB;Persist Security Info=False;User ID=AlphaSweater;Password=N@l@2004;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static SqlConnection con = new SqlConnection(conString);

        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? PhoneNo { get; set; }

        public int insert_User(tblUsers m)
        {
            try
            {
                string sql = "INSERT INTO UserTBL (UserName, UserSurname, UserPhoneNo) VALUES (@UserName, @UserSurname, @UserPhoneNo)";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@UserName", m.Name);
                cmd.Parameters.AddWithValue("@UserSurname", m.Surname);
                cmd.Parameters.AddWithValue("@UserPhoneNo", m.PhoneNo);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();
                return rowsAffected;
            } catch (System.Exception)
            {
                throw;
            }
        }
    }
}