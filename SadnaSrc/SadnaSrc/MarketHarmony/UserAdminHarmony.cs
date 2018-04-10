using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.AdminView;
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
        public void ValidateSystemAdmin()
        {
            if (user != null && user.IsSystemAdmin())
            {
                return;
            }

            throw new UserException(ManageMarketSystem.NotSystemAdmin,
                "User which hasn't fully identified as System Admin cannot act as one!");
        }

        public int GetAdminSystemID()
        {
            if (user != null)
            {
                return user.SystemID;
            }

            return -1;
        }

        public string GetAdminName()
        {
            if (user != null && user.IsRegisteredUser())
            {
                return ((RegisteredUser) user).Name;
            }

            return null;
        }
    }
}
