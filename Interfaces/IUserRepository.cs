using CloudDevPOE.Models;

namespace CloudDevPOE.Interfaces
{
    public interface IUserRepository
    {
        bool Validate_User(tbl_users user);

        int Insert_User(tbl_users user);
    }
}