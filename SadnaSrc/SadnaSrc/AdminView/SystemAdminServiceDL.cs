using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;

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

        public void IsUserExist(string userName)
        {
            using (var dbReader = SelectFromTableWithCondition("User", "*", "Name = '" + userName +"'"))
            {
                if (!dbReader.Read())
                {
                    throw new AdminException(RemoveUserStatus.NoUserFound,"Couldn't find any User with that Name to remove");
                }
            }
        }

        public void IsUserNameExistInHistory(string userName)
        {
            using (var dbReader = SelectFromTableWithCondition("PurchaseHistory", "*", "UserName = '" + userName +"'"))
            {
                if (!dbReader.Read())
                {
                    throw new AdminException(ViewPurchaseHistoryStatus.NoUserFound, "Couldn't find any User with that ID in history records");
                }
            }
        }

        public void IsStoreExistInHistory(string storeName)
        {
            using (var dbReader = SelectFromTableWithCondition("PurchaseHistory", "*", "Store = '" + storeName +"'"))
            {
                if (!dbReader.Read())
                {
                    throw new AdminException(ViewPurchaseHistoryStatus.NoStoreFound, "Couldn't find any Store with that name in history records");
                }
            }
        }

        public void DeleteUser(string userName)
        {
            DeleteFromTable("User", "Name = '" + userName +"'");
        }

        private string[] GetPurchaseHistory(SQLiteDataReader dbReader)
        {
            List<string> historyData = new List<string>();
            while (dbReader.Read())
            {
                PurchaseHistory record = new PurchaseHistory(dbReader.GetString(0), dbReader.GetString(1),
                    dbReader.GetString(2),dbReader.GetString(3),dbReader.GetInt32(4),dbReader.GetDouble(5), dbReader.GetString(6));
                historyData.Add(record.ToString());
            }

            return historyData.ToArray();
        }
        public string[] GetPurchaseHistory(string field, string givenValue)
        {
            using (var dbReader = SelectFromTableWithCondition("PurchaseHistory", "*", field + " = '" + givenValue + "'"))
            {
                return GetPurchaseHistory(dbReader);
            }
        }

    }
}
