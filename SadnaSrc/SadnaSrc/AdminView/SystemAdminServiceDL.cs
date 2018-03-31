using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SadnaSrc.Main;

namespace SadnaSrc.AdminView
{
    class SystemAdminServiceDL : SystemDL
    {

        public string[] FindSolelyOwnedStores()
        {
            List<string> solelyOwnedStores = new List<string>();
            string cmd = @"SELECT Name FROM Store AS T1 LEFT JOIN
                        (SELECT Store FROM StoreManagerPolicy 
                        WHERE Action = 'StoreOwner') AS T2 ON T1.Name = T2.Store
                        WHERE T2.Store IS NULL";

            using (var dbReader = freeStyleSelect(cmd))
            {
                while(dbReader.Read())
                {
                    solelyOwnedStores.Add(dbReader.GetString(0));
                }
            }

            return solelyOwnedStores.ToArray();

        }

        public void CloseStore(string store)
        {
            UpdateTable("Store", "Name = '"+store+"'",new[] {"Status"},new[] {"@stat"},new object[] {"Inactive"});
        }

        public void IsUserExist(int userSystemID)
        {
            using (var dbReader = SelectFromTableWithCondition("User", "SystemID", "SystemID = " + userSystemID))
            {
                if (!dbReader.Read())
                {
                    throw new AdminException(RemoveUserStatus.NoUserFound,"Couldn't find any User with that ID to remove");
                }
            }
        }

        public void DeleteUser(int toDeleteID)
        {
            DeleteFromTable("User", "SystemID = " + toDeleteID);
        }
    }
}
