using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.MarketHarmony
{
    class UserAdmin : IUserAdmin
    {
        public UserAdmin(IUserService userService)
        {

        }

        public int GetAdminSystemID()
        {
            throw new NotImplementedException();
        }

        public bool IsSystemAdmin()
        {
            throw new NotImplementedException();
        }
    }
}
