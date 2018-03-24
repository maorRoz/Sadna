using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    class UserService : IUserService,ISystemAdminService
    {
        private User user;
        private UserServiceDL _userDL;
        private CartService cart;
        private int systemID;

        public UserService(SQLiteConnection dbConnection)
        {
            var random = new Random();
            _userDL = new UserServiceDL(dbConnection);
            systemID = _userDL.getSystemID();
            UserException.SetUser(systemID);
            UserPolicyService.EstablishServiceDL(_userDL);
            CartService.EstablishServiceDL(_userDL);
        }
        public void EnterSystem()
        {
              user = new User(systemID);
              cart = new CartService(false);
        }
    }
}
