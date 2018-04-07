using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace SadnaSrc.MarketHarmony
{
    public class UserAdminHarmony : IUserAdmin
    {
        private readonly User user;
        public UserAdminHarmony(IUserService userService)
        {
            user = ((UserService)userService).MarketUser;
        }
        public bool IsSystemAdmin()
        {
            return user != null && user.IsSystemAdmin();
        }

        public int GetAdminSystemID()
        {
            return user.SystemID;
        }

        public string GetAdminName()
        {
            return ((RegisteredUser)user).Name;
        }
    }
}
