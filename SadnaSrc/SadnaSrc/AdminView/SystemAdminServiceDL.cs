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
        private int SystemID { get; set; }
        public SystemAdminServiceDL(SQLiteConnection dbConnection) : base(dbConnection)
        {
        }

        public string[] FindSolelyOwnedStores()
        {
            List<string> solelyOwnedStores = new List<string>();
            string cmd1 = @"SELECT Store FROM 
                        (SELECT Count(Store) as OwnersCount,Store FROM StoreManagerPolicy 
                        WHERE Action = 'StoreOwner' GROUP BY Store) as StoreOwnersNum
                        WHERE StoreOwnersNum.OwnersCount = 0";
            // if store doesn't exist in the table store manager policy , you wouldn't find it in the table
            // you need to do a join with store table.

            using (var dbReader = freeStyleSelect(cmd1))
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
            UpdateTable("Store", "Name = "+store,new[] { "Status"},new[] {"@stat"},new object[] {"Inactive"});
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
