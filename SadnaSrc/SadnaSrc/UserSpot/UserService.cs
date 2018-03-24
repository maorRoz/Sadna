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

        public UserService(SQLiteConnection dbConnection)
        {
            _userDL = new UserServiceDL(dbConnection,0);
            UserPolicyService.EstablishServiceDL(_userDL);
            CartService.EstablishServiceDL(_userDL);
        }
        public void EnterSystem()
        {
            throw new NotImplementedException();
        }
    }
}
