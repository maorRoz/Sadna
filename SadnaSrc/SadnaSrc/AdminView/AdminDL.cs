using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.AdminView
{
    class AdminDL : IAdminDL
    {

        private static AdminDL _instance;

        public static AdminDL Instance => _instance ?? (_instance = new AdminDL());

        private MarketDB dbConnection;
        private AdminDL()
        {
            dbConnection = MarketDB.Instance;
        }
        public string[] FindSolelyOwnedStores()
        {
            List<string> solelyOwnedStores = new List<string>();
            string cmd = @"SELECT Name FROM Store AS T1 LEFT JOIN
                        (SELECT Store FROM StoreManagerPolicy 
                        WHERE Action = 'StoreOwner') AS T2 ON T1.Name = T2.Store
                        WHERE T2.Store IS NULL";

            using (var dbReader = dbConnection.freeStyleSelect(cmd))
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
            dbConnection.UpdateTable("Store", "Name = '"+store+"'",new[] {"Status"},new[] {"@stat"},new object[] {"Inactive"});
        }

        public bool IsUserExist(string userName)
        {
            using (var dbReader = dbConnection.SelectFromTableWithCondition("User", "*", "Name = '" + userName +"'"))
            {
                return dbReader.Read();
            }
        }

        public bool IsUserNameExistInHistory(string userName)
        {
            using (var dbReader = dbConnection.SelectFromTableWithCondition("PurchaseHistory", "*", "UserName = '" + userName +"'"))
            {
                return dbReader.Read();
            }
        }

        public bool IsStoreExistInHistory(string storeName)
        {
            using (var dbReader = dbConnection.SelectFromTableWithCondition("PurchaseHistory", "*", "Store = '" + storeName +"'"))
            {
                return dbReader.Read();
            }
        }

        public void DeleteUser(string userName)
        {
            dbConnection.DeleteFromTable("User", "Name = '" + userName +"'");
        }

        private string[] GetPurchaseHistory(SQLiteDataReader dbReader)
        {
            List<string> historyData = new List<string>();
            while (dbReader.Read())
            {
                PurchaseHistory record = new PurchaseHistory(dbReader.GetString(0), dbReader.GetString(1),
                    dbReader.GetString(2),dbReader.GetString(3),dbReader.GetInt32(4),dbReader.GetDouble(5),
                    dbReader.GetString(6));
                historyData.Add(record.ToString());
            }

            return historyData.ToArray();
        }
        public string[] GetPurchaseHistory(string field, string givenValue)
        {
            using (var dbReader = dbConnection.SelectFromTableWithCondition("PurchaseHistory", "*", field + " = '"
                                                                                                          + givenValue + "'"))
            {
                return GetPurchaseHistory(dbReader);
            }
        }

    }
}
