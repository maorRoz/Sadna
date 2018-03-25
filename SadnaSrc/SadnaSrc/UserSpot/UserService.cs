using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    public class UserService : IUserService,ISystemAdminService
    {
        private User user;
        private readonly UserServiceDL userDL;
        private CartService cart;
        private int systemID;

        public UserService(SQLiteConnection dbConnection)
        {
            var random = new Random();
            userDL = new UserServiceDL(dbConnection);
            systemID = userDL.GetSystemID();
            UserException.SetUser(systemID);
            UserPolicyService.EstablishServiceDL(userDL);
            CartService.EstablishServiceDL(userDL);
        }
        public void EnterSystem()
        {
            user = new User(systemID);
            cart = new CartService(false);
            MarketLog.Log("UserSpot","User "+systemID+" has entered the system");
        }

        public User GetUser()
        {
            return user;
        }

        public void ExitSystem()
        {
            userDL.DeleteUser();
        }
    }
}
