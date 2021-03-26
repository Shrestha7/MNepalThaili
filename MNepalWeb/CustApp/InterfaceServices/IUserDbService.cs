using CustApp.Models;
using System.Data;

namespace CustApp.InterfaceServices
{
    public interface IUserDbService
    {
        DataTable GetUserInformation(UserInfo objUserInfo);
    }
}
