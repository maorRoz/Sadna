using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    class SystemAdminDL : systemDL
    {
        public SystemAdminDL(SQLiteConnection dbConnection) : base(dbConnection)
        {
        }

        public void RemoveUser(User user)
        {

        }

        public List<string> ViewUserPurchaseHistory(User user)
        {
            return null;
        }

        public List<string> ViewStorePurchaseHistory(string store)
        {
            return null;
        }
    }
}
