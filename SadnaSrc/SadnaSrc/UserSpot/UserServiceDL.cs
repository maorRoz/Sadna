﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    class UserServiceDL : systemDL
    {
        private int _systemID;
        public UserServiceDL(SQLiteConnection dbConnection) : base(dbConnection)
        {
        }

        private List<int> GetAllSystemIDs()
        {
            var ids = new List<int>();
            var dbReader = SelectFromTable("User", "SystemID");
            while (dbReader.Read())
            {
                if (dbReader.GetValue(0) != null)
                {
                    ids.Add(dbReader.GetInt32(0));
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

            _systemID = newID;
        }

        public int getSystemID()
        {
            GenerateSystemID();
            SaveUser(new User(_systemID));
            return _systemID;
        }
        public void SaveUserPolicy(UserPolicy policy)
        {
            string [] valuesNames = {"@idParam","@stateParam","@actionParam","@storeParam"};
            object[] values = { _systemID, policy.GetStateString(), null, null};
            InsertTable("UserPolicy", "SystemID,state,action,store",valuesNames,values);
        }

        public void SaveUserPolicy(StoreAdminPolicy policy)
        {

        }

        public void DeleteUserPolicy(UserPolicy policy)
        {

        }

        public void DeleteUserPolicy(StoreAdminPolicy policy)
        {

        }

        private UserPolicy LoadUserPolicy()
        {
            return null;
        }
        private void EncryptPassword()
        {

        }

        private void DecryptPassword()
        {

        }
        public void SaveUser(User user)
        {
            string[] valuesNames = { "@idParam", "@nameParam", "@addressParam", "@passParam" };
            object[] values = user.ToData();
            InsertTable("User", "SystemID,Name,Address,Password", valuesNames,values);
        }
        public User LoadUser()
        {
            return null;
        }

        public void SaveCartItem(CartItem item)
        {

        }

        public void RemoveCartItem(CartItem item)
        {

        }
        private List<CartItem> LoadCartItems()
        {
            return null;
        }

        public void UpdateCartItemQuantity(int quantity)
        {

        }
    }
}
