﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    public class UserServiceDL : SystemDL
    {

        private int SystemID { get; set; }


        private List<int> GetAllSystemIDs()
        {
            var ids = new List<int>();
            using (var dbReader = SelectFromTable("User", "SystemID"))
            {
                while (dbReader.Read())
                {
                    if (dbReader.GetValue(0) != null)
                    {
                        ids.Add(dbReader.GetInt32(0));
                    }
                }
            }

            return ids;
        }
        private void GenerateSystemID()
        {
            int newID = new Random().Next(1000, 10000);
            List<int> savedIDs = GetAllSystemIDs();
            while (savedIDs.Contains(newID))
            {
                newID = new Random().Next(1000, 10000);
            }

            SystemID = newID;
        }

        public int GetSystemID()
        {
            GenerateSystemID();
            SaveUser(new User(SystemID));
            return SystemID;
        }

        public bool IsUserNameExist(string name)
        {
            using (var dbReader = SelectFromTableWithCondition("User", "*", "name = '" + name + "'"))
            {
                return dbReader.Read();

            }
        }
        public RegisteredUser RegisterUser(string name, string address, string password, CartItem[] guestCart)
        {
            if (IsUserNameExist(name))
            {
                throw new UserException(SignUpStatus.TakenName,"register action has been requested while there" +
                                        " is already a User with the given name in the system!");
            }
            string[] columnNames = { "Name" , "Address" , "Password" };
            string[] valuesNames = {"@name", "@address", "@password"};
            object[] values = {name, address, password};
            UpdateTable("User","SystemID = "+SystemID, columnNames ,valuesNames,values);
            SaveCartItem(guestCart);
            return new RegisteredUser(SystemID, name,address,password,guestCart);
        }
        public void SaveUserStatePolicy(StatePolicy policy)
        {
            string [] valuesNames = {"@idParam","@stateParam"};
            object[] values = { SystemID, policy.GetStateString()};
            InsertTable("StatePolicy", "SystemID,State",valuesNames,values);
        }

        private int GetUserNameFromID(string userName)
        {
            var dbReader = SelectFromTableWithCondition("User","SystemID", "WHERE Name = '" + userName + "'");
            if (dbReader.Read())
            {
                return dbReader.GetInt32(0);
            }

            throw new UserException(MarketError.DbError,
                "No user by the name " + userName + " has been found in the db");
        }

        public void SaveUserStorePolicy(string userName,StoreManagerPolicy policy)
        {
            int idOfPromoted = GetUserNameFromID(userName);
            string[] valuesNames = { "@idParam", "@storeParam","@actionParam" };
            object[] values = { idOfPromoted, policy.Store,policy.GetStoreActionString() };
            InsertTable("StoreManagerPolicy", "SystemID,Store,Action", valuesNames, values);
        }

        public void SaveUserStorePolicy(StoreManagerPolicy policy)
        {
            string[] valuesNames = { "@idParam", "@storeParam", "@actionParam" };
            object[] values = { SystemID, policy.Store, policy.GetStoreActionString() };
            InsertTable("StoreManagerPolicy", "SystemID,Store,Action", valuesNames, values);
        }

        public void DeleteUserStorePolicy(string userName, StoreManagerPolicy policy)
        {
            int idOfDemoted = GetUserNameFromID(userName);
            DeleteFromTable("StoreManagerPolicy","SystemID = "+ idOfDemoted + " AND Store = "+policy.Store
                                                 + " AND Action =" + policy.Action);
        }

        private StatePolicy[] LoadUserStatePolicy()
        {
            List<StatePolicy> loadedStatesPolicies = new List<StatePolicy>();
            using (var dbReader = SelectFromTableWithCondition("StatePolicy", "State", "SystemID = " + SystemID))
            {
                while (dbReader.Read())
                {
                    StatePolicy.State state = StatePolicy.GetStateFromString(dbReader.GetString(0));
                    loadedStatesPolicies.Add(new StatePolicy(state));
                }
            }
            return loadedStatesPolicies.ToArray();
        }

        private StoreManagerPolicy[] LoadUserStorePolicies()
        {
            List<StoreManagerPolicy> loadedStorePolicies = new List<StoreManagerPolicy>();
            using (var dbReader = SelectFromTableWithCondition("StoreManagerPolicy", "*", "SystemID = " + SystemID))
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
        public void SaveUser(User user)
        {
            InsertTable("User", "SystemID,Name,Address,Password",
                new [] { "@idParam", "@nameParam", "@addressParam", "@passParam" }, user.ToData());
        }

        private string[] UserNamesInSystem()
        {
            List<string> userNames = new List<string>();
            using (var dbReader = SelectFromTable("User", "Name"))
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

        private bool IsSimilar(string str1, string str2)
        {
            if (str1.Length != str2.Length)
            {
                return false;
            }

            int same = 0;
            for (int i = 0; i < str1.Length; i++)
            {
                if (str1[i] != str2[i])
                {
                    same--;
                }
            }
            return same >= -2;
        }
        private string FindSimilar(string name)
        {
            string[] userNames = UserNamesInSystem();
            string similarName = "";
            foreach (string userName in userNames)
            {
                if (name.Equals(userName))
                {
                    similarName = "";
                    break;
                }

                if (IsSimilar(name, userName))
                {
                    similarName = userName;
                }
            }

            return similarName;
        }

        private object[] FindRegisteredUserData(string name, string password)
        {
            using (var dbReader = SelectFromTableWithCondition("User", "*", "name = '" + name + "' AND password = '"+ password +"'"))
            {
                while (dbReader.Read())
                {
                    return new object[] {dbReader.GetInt32(0), dbReader.GetString(2)};
                }

                string similarName = FindSimilar(name);
                if (similarName.Length > 0)
                {
                    throw new UserException(SignInStatus.MistakeTipGiven, "No user were found by that name, " +
                                                                      "have you meant to enter " + similarName + "?");
                }
                throw new UserException(SignInStatus.NoUserFound,"sign in action has been requested while there" +
                                        " is no User with the given name or password in the system! ");

            }
        }
        public RegisteredUser LoadUser(string name, string password, CartItem[] guestCart)
        {
            object[] loadedUserIdAndAddress = FindRegisteredUserData(name, password);

            SystemID = (int) loadedUserIdAndAddress[0];
            SaveCartItem(guestCart);

            return new RegisteredUser(SystemID,name,(string) loadedUserIdAndAddress[1],
                password, LoadCartItems(), LoadUserStatePolicy(), LoadUserStorePolicies());
        }

        public void RemoveCart()
        {
            DeleteFromTable("CartItem","SystemID = "+SystemID);
        }

        public void SaveCartItem(CartItem[] cart)
        {
            foreach (CartItem item in cart)
            {
                var userItem = new List<object>();
                userItem.Add(SystemID);
                userItem.AddRange(item.ToData());
                InsertTable("CartItem", "SystemID,Name,Store,Quantity,UnitPrice,FinalPrice,SaleType",
                    new [] { "@idParam", "@nameParam", "@storeParam","@quantityParam","@unitpriceParam","@finalpriceParam",
                        "@saleParam" }, userItem.ToArray());
            }
        }

        public void RemoveCartItem(CartItem item)
        {
            DeleteFromTable("CartItem", "SystemID = "+SystemID +" AND "+ item.GetDbIdentifier());
        }
        private CartItem[] LoadCartItems()
        {
            List<CartItem> loadedItems = new List<CartItem>();
            using (var dbReader = SelectFromTableWithCondition("CartItem", "*", "SystemID = " + SystemID))
            {
                while (dbReader.Read())
                {
                    loadedItems.Add(new CartItem(dbReader.GetString(1),
                        dbReader.GetString(2),dbReader.GetInt32(3),dbReader.GetDouble(4),dbReader.GetString(6)));
                }
            }
            return loadedItems.ToArray();
        }

        public void UpdateCartItemQuantity(CartItem item)
        {
            string[] columnNames = { "Quantity", "FinalPrice"};
            string[] valuesNames = { "@quantity", "@price"};
            object[] values = { item.Quantity,item.FinalPrice};
            UpdateTable("CartItem", item.GetDbIdentifier(), columnNames, valuesNames, values);
        }

        public void DeleteUser(int toDeleteID)
        {
            DeleteFromTable("User", "SystemID = " + toDeleteID);
        }

    }
}
