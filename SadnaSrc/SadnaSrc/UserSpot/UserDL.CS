using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    public class UserDL : IUserDL
    {

        private static UserDL _instance;

        public static UserDL Instance => _instance ?? (_instance = new UserDL());

        private static List<int> userIDs = new List<int>();

        private readonly MarketDB dbConnection;

        private UserDL()
        {
            dbConnection = MarketDB.Instance;
        }

        public int[] GetAllSystemIDs()
        {
            var ids = new List<int>();
            using (var dbReader = dbConnection.SelectFromTable("User", "SystemID"))
            {
                while (dbReader.Read())
                {
                    if (dbReader.GetValue(0) != null)
                    {
                        ids.Add(dbReader.GetInt32(0));
                    }
                }
            }

            return ids.ToArray();
        }

        public bool IsUserNameExist(string name)
         {
            using (var dbReader = dbConnection.SelectFromTableWithCondition("User", "*", "Name = '" + name + "'"))
            {
                return dbReader.Read();

            }
        }
        public RegisteredUser RegisterUser(int userID,string name, string address, string password, string creditCard, CartItem[] guestCart)
        {
            string[] columnNames = { "Name" , "Address" , "Password","CreditCard" };
            string[] valuesNames = {"@name", "@address", "@password","@card"};
            object[] values = {name, address, password,creditCard};
            dbConnection.UpdateTable("User","SystemID = "+ userID, columnNames ,valuesNames,values);
            SaveCartItem(userID,guestCart);
            return new RegisteredUser(userID, name,address,password,creditCard,guestCart);
        }
        public void SaveUserStatePolicy(int userID,StatePolicy policy)
        {
            string [] valuesNames = {"@idParam","@stateParam"};
            object[] values = { userID, policy.GetStateString()};
            dbConnection.InsertTable("StatePolicy", "SystemID,State",valuesNames,values);
        }

        private int GetIDFromUserName(string userName)
        {
            using (var dbReader = dbConnection.SelectFromTableWithCondition("User", "SystemID", "Name = '" + userName + "'"))
            {
                if (dbReader.Read())
                {
                    return dbReader.GetInt32(0);
                }
                return -1;
            }
        }

        public void SaveUserStorePolicy(string userName,StoreManagerPolicy policy)
        {
            int idOfPromoted = GetIDFromUserName(userName);
            string[] valuesNames = { "@idParam", "@storeParam","@actionParam" };
            object[] values = { idOfPromoted, policy.Store,policy.GetStoreActionString() };
            dbConnection.InsertTable("StoreManagerPolicy", "SystemID,Store,Action", valuesNames, values);
        }

        public void SaveUserStorePolicy(int userID,StoreManagerPolicy policy)
        {
            string[] valuesNames = { "@idParam", "@storeParam", "@actionParam" };
            object[] values = { userID, policy.Store, policy.GetStoreActionString() };
            dbConnection.InsertTable("StoreManagerPolicy", "SystemID,Store,Action", valuesNames, values);
        }

        public void DeleteUserStorePolicy(string userName, StoreManagerPolicy policy)
        {
            int idOfDemoted = GetIDFromUserName(userName);
            dbConnection.DeleteFromTable("StoreManagerPolicy","SystemID = "+ idOfDemoted + " AND Store = '"+policy.Store
                                                 + "' AND Action = '" + policy.Action +"'");
        }

        private StatePolicy[] LoadUserStatePolicy(int userID)
        {
            List<StatePolicy> loadedStatesPolicies = new List<StatePolicy>();
            using (var dbReader = dbConnection.SelectFromTableWithCondition("StatePolicy", "State", "SystemID = " + userID))
            {
                while (dbReader.Read())
                {
                    StatePolicy.State state = StatePolicy.GetStateFromString(dbReader.GetString(0));
                    loadedStatesPolicies.Add(new StatePolicy(state));
                }
            }
            return loadedStatesPolicies.ToArray();
        }

        private StoreManagerPolicy[] LoadUserStorePolicies(int userID)
        {
            List<StoreManagerPolicy> loadedStorePolicies = new List<StoreManagerPolicy>();
            using (var dbReader = dbConnection.SelectFromTableWithCondition("StoreManagerPolicy", "*", "SystemID = " + userID))
            {
                while (dbReader.Read())
                {
                    string storeName = dbReader.GetString(1);
                    StoreManagerPolicy.StoreAction action =
                        StoreManagerPolicy.GetActionFromString(dbReader.GetString(2));
                    loadedStorePolicies.Add(new StoreManagerPolicy(storeName,action));

                }
            }
            return loadedStorePolicies.ToArray();
        }
        public virtual void SaveUser(User user)
        {
            dbConnection.InsertTable("User", "SystemID,Name,Address,Password,CreditCard",
                new [] { "@idParam", "@nameParam", "@addressParam", "@passParam","@creditParam" }, user.ToData());
            userIDs.Add(user.SystemID);
        }

        public string[] UserNamesInSystem()
        {
            var userNames = new List<string>();
            using (var dbReader = dbConnection.SelectFromTable("User", "Name"))
            {
                while (dbReader.Read())
                {
                    if (!dbReader.IsDBNull(0))
                    {
                        userNames.Add(dbReader.GetString(0));
                    }
                }

            }
            return userNames.ToArray();
        }

        public object[] FindRegisteredUserData(string name, string password)
        {
            using (var dbReader = dbConnection.SelectFromTableWithCondition("User", "*", "name = '" + name + "' AND password = '"+ password +"'"))
            {
                if(dbReader.Read())
                {
                    return new object[] {dbReader.GetInt32(0),dbReader.GetString(1), dbReader.GetString(2),
                        dbReader.GetString(3),dbReader.GetString(4)};
                }

                return null;
            }
        }
        public RegisteredUser LoadUser(object[] userData, CartItem[] guestCart)
        {
            var loadedID = (int)userData[0];
            var loadedName = (string)userData[1];
            var loadedAddress = (string)userData[2];
            var loadedPassword = (string)userData[3];
            var loadedCreditCard = (string)userData[4];
            SaveCartItem(loadedID, guestCart);

            return new RegisteredUser(loadedID, loadedName, loadedAddress, loadedPassword, loadedCreditCard, 
                LoadCartItems(loadedID), LoadUserStatePolicy(loadedID), LoadUserStorePolicies(loadedID));
        }

        public void RemoveCart(int userID)
        {
            dbConnection.DeleteFromTable("CartItem","SystemID = "+ userID);
        }

        public void SaveCartItem(int userID,CartItem[] cart)
        {
            foreach (CartItem item in cart)
            {
                var userItem = new List<object>();
                userItem.Add(userID);
                userItem.AddRange(item.ToData());
                dbConnection.InsertTable("CartItem", "SystemID,Name,Store,Quantity,UnitPrice,FinalPrice",
                    new [] { "@idParam", "@nameParam", "@storeParam","@quantityParam","@unitpriceParam","@finalpriceParam"}, userItem.ToArray());
            }
        }

        public void RemoveCartItem(int userID,CartItem item)
        {
            dbConnection.DeleteFromTable("CartItem", "SystemID = "+ userID + " AND "+ item.GetDbIdentifier());
        }
        private CartItem[] LoadCartItems(int userID)
        {
            List<CartItem> loadedItems = new List<CartItem>();
            using (var dbReader = dbConnection.SelectFromTableWithCondition("CartItem", "*", "SystemID = " + userID))
            {
                while (dbReader.Read())
                {
                    loadedItems.Add(new CartItem(dbReader.GetString(1),
                        dbReader.GetString(2),dbReader.GetInt32(3),dbReader.GetDouble(4)));
                }
            }
            return loadedItems.ToArray();
        }

        public void UpdateCartItemQuantity(CartItem item)
        {
            string[] columnNames = { "Quantity", "FinalPrice"};
            string[] valuesNames = { "@quantity", "@price"};
            object[] values = { item.Quantity,item.FinalPrice};
            dbConnection.UpdateTable("CartItem", item.GetDbIdentifier(), columnNames, valuesNames, values);
        }

        public void CleanSession()
        {
            foreach(var userID in userIDs)
            {
                dbConnection.DeleteFromTable("User", "SystemID = " + userID);
            }
        }

    }
}
