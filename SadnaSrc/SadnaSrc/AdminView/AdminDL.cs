using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;
using SadnaSrc.MarketRecovery;

namespace SadnaSrc.AdminView
{
    public class AdminDL : IAdminDL
    {

        private static AdminDL _instance;

        public static AdminDL Instance => _instance ?? (_instance = new AdminDL());

        private readonly IMarketDB dbConnection;
        private readonly IMarketBackUpDB dbBackupConnection;
        private AdminDL()
        {
            dbConnection = new ProxyMarketDB();
            dbBackupConnection = MarketBackUpDB.Instance;
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

	    public string[] GetAllUserInPurchaseHistory()
	    {
			LinkedList<string> users = new LinkedList<string>();
			using (var dbReader = dbConnection.SelectFromTable("PurchaseHistory", "UserName"))
			{
				while (dbReader.Read())
				{
					users.AddLast(dbReader.GetString(0));
				}
				
			}

		    return users.ToArray();
	    }

        public string[] GetEventLogReport()
        {
            var logEntries = new List<string>();
            using (var dbReader = dbBackupConnection.SelectFromTable("System_Log", "*"))
            {
                while (dbReader.Read())
                {
                    var entry = "ID: " + dbReader.GetString(0) + " Date: "
                                + dbReader.GetDateTime(1) + " Type: " +
                                dbReader.GetString(2) + " Description: " + dbReader.GetString(3);
                    logEntries.Add(entry);
                }
            }

            return logEntries.ToArray();
        }

        public string[] GetEventErrorLogReport()
        {
            var errorEntries = new List<string>();
            using (var dbReader = dbBackupConnection.SelectFromTable("System_Errors", "*"))
            {
                while (dbReader.Read())
                {
                    var entry = "ID: " + dbReader.GetString(0) + " Date: " 
                                + dbReader.GetDateTime(1) + " Type: " +
                          dbReader.GetString(2) + " Description: " + dbReader.GetString(3);
                    errorEntries.Add(entry);
                }
            }

            return errorEntries.ToArray();
        }

        public string[] GetAllStoresInPurchaseHistory()
	    {
		    LinkedList<string> stores = new LinkedList<string>();
		    using (var dbReader = dbConnection.SelectFromTable("PurchaseHistory", "Store"))
		    {
			    while (dbReader.Read())
			    {
				    stores.AddLast(dbReader.GetString(0));
				}
					
		    }

		    return stores.ToArray();
		}

        public void CloseStore(string store)
        {
            CheckInput(store);
            dbConnection.UpdateTable("Store", "Name = '"+store+"'",new[] {"Status"},new[] {"@stat"},new object[] {"Inactive"});
        }

        public bool IsUserExist(string userName)
        {
            CheckInput(userName);
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Users", "*", "Name = '" + userName +"'"))
            {
                return dbReader.Read();
            }
        }

        public bool IsUserNameExistInHistory(string userName)
        {
            CheckInput(userName);
            using (var dbReader = dbConnection.SelectFromTableWithCondition("PurchaseHistory", "*", "UserName = '" + userName +"'"))
            {
                return dbReader.Read();
            }
        }

        public bool IsStoreExistInHistory(string storeName)
        {
            CheckInput(storeName);
            using (var dbReader = dbConnection.SelectFromTableWithCondition("PurchaseHistory", "*", "Store = '" + storeName +"'"))
            {
                return dbReader.Read();
            }
        }

        public void DeleteUser(string userName)
        {
            dbConnection.DeleteFromTable("Users", "Name = '" + userName +"'");
        }

        private string[] GetPurchaseHistory(SqlDataReader dbReader)
        {
            List<string> historyData = new List<string>();
            while (dbReader.Read())
            {
                PurchaseHistory record = new PurchaseHistory(dbReader.GetString(0), dbReader.GetString(1),
                    dbReader.GetString(2),dbReader.GetString(3),dbReader.GetInt32(4),dbReader.GetDouble(5),
                    dbReader.GetDateTime(6).ToString());
                historyData.Add(record.ToString());
            }

            return historyData.ToArray();
        }
        public string[] GetPurchaseHistory(string field, string givenValue)
        {
            CheckInput(field); CheckInput(givenValue);
            using (var dbReader = dbConnection.SelectFromTableWithCondition("PurchaseHistory", "*", field + " = '"
                                                                                                          + givenValue + "'"))
            {
                return GetPurchaseHistory(dbReader);
            }
        }
        public void AddCategory(Category category)
        {
            foreach (object val in category.GetCategoryValuesArray())
                CheckInput(val.ToString());

            dbConnection.InsertTable("Category", "SystemID, name",
                new[]{"@idParam","@nameParam"}, category.GetCategoryValuesArray());
        }
        public void RemoveCategory(Category category)
        {
            CheckInput(category.SystemId);
            dbConnection.DeleteFromTable("Category", "SystemID = '" + category.SystemId + "'");
        }
        public Category GetCategoryByName(string categoryname)
        {
            CheckInput(categoryname);
            Category category = null;
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("Category", "*", "name = '" + categoryname + "'"))
            {
                while (dbReader.Read())
                {
                    category = new Category(dbReader.GetString(0), dbReader.GetString(1));
                }
            }
            return category;
        }

        private void CheckInput(string input)
        {
            if (input.IndexOf("'") != -1)
                throw new DataException("Input value can't contain char ' ");
        }

    }
}
