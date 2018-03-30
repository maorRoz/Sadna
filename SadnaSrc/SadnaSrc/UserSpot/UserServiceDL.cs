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
        public UserServiceDL(SQLiteConnection dbConnection) : base(dbConnection)
        {
        }

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


        private bool IsUserNameExist(string name)
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
        public void SaveUserPolicy(UserPolicy policy)
        {
            string [] valuesNames = {"@idParam","@stateParam","@actionParam","@storeParam"};
            object[] values = { SystemID, policy.GetStateString(), null, null};
            InsertTable("UserPolicy", "SystemID,state,action,store",valuesNames,values);
        }

        public void SaveUserPolicy(StoreManagerPolicy policy)
        {

        }
 
        public void DeleteUserPolicy(StoreManagerPolicy policy)
        {

        }

        private UserPolicy[] LoadUserPolicy()
        {
            List<UserPolicy> loadedPolicies = new List<UserPolicy>();
            using (var dbReader = SelectFromTableWithCondition("UserPolicy", "*", "SystemID = " + SystemID))
            {
                while (dbReader.Read())
                {
                    if (dbReader.GetString(1).Equals("RegisteredUser"))
                    {
                        loadedPolicies.Add(new UserPolicy(UserPolicy.State.RegisteredUser));
                    }
                    else if (dbReader.GetString(1).Equals("SystemAdmin"))

                    {
                        loadedPolicies.Add(new UserPolicy(UserPolicy.State.SystemAdmin));
                    }
                    else
                    {
                        loadedPolicies.Add(new StoreManagerPolicy(StoreManagerPolicy.GetActionFromString(
                            dbReader.GetString(2)), dbReader.GetString(3)));
                    }
                }
            }
            return loadedPolicies.ToArray();
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
            foreach (CartItem item in guestCart)
            {
                item.SetUserID(SystemID);
            }
            SaveCartItem(guestCart);

            return new RegisteredUser(SystemID,name,(string) loadedUserIdAndAddress[1],
                password, LoadCartItems(), LoadUserPolicy());
        }

        public void SaveCartItem(CartItem[] cart)
        {
            foreach (CartItem item in cart)
            {
                InsertTable("CartItem", "SystemID,Name,Store,Quantity,FinalPrice,SaleType",
                    new [] { "@idParam", "@nameParam", "@storeParam", "@priceParam", "@saleParam" }, item.ToData());
            }
        }

        public void RemoveCartItem(CartItem item)
        {

        }
        private CartItem[] LoadCartItems()
        {
            List<CartItem> loadedItems = new List<CartItem>();
            using (var dbReader = SelectFromTableWithCondition("CartItem", "*", "SystemID = " + SystemID))
            {
                while (dbReader.Read())
                {
                    loadedItems.Add(new CartItem(dbReader.GetInt32(0),dbReader.GetString(1),
                        dbReader.GetString(2),dbReader.GetInt32(3),dbReader.GetDouble(4),dbReader.GetString(5)));
                }
            }
            return loadedItems.ToArray();
        }

        public void UpdateCartItemQuantity(int quantity)
        {

        }

        public void DeleteUser(int toDeleteID)
        {
            DeleteFromTable("User", "SystemID = " + toDeleteID);
        }

    }
}
