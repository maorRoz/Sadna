using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    public class UserServiceDL : systemDL
    {
        private int _systemID;
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

            _systemID = newID;
        }

        public int GetSystemID()
        {
            GenerateSystemID();
            SaveUser(new User(_systemID));
            return _systemID;
        }


        private bool IsUserExist(string name)
        {
            using (var dbReader = SelectFromTableWithCondition("User", "*", "name = '" + name + "'"))
            {
                return dbReader.Read();

            }
        }
        public RegisteredUser RegisterUser(string name, string address, string password, CartItem[] guestCart)
        {
            if (IsUserExist(name))
            {
                throw new UserException("register action has been request while there" +
                                        " is already a User with the given name in the system!");
            }
            string[] columnNames = { "Name" , "Address" , "Password" };
            string[] valuesNames = {"@name", "@address", "@password"};
            object[] values = {name, address, password};
            UpdateTable("User","SystemID = "+_systemID, columnNames ,valuesNames,values);
            SaveCartItem(guestCart);
            return new RegisteredUser(_systemID, name,address,password,guestCart);
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
        public void SaveUser(User user)
        {
            string[] valuesNames = { "@idParam", "@nameParam", "@addressParam", "@passParam" };
            object[] values = user.ToData();
            InsertTable("User", "SystemID,Name,Address,Password", valuesNames,values);
        }
        public RegisteredUser LoadUser(string name, string password, CartItem[] guestCart)
        {
            return null;
        }

        public void DeleteUser()
        {
            DeleteFromTable("User", "SystemID = "+_systemID);
        }

        public void SaveCartItem(CartItem[] cart)
        {
            foreach (CartItem item in cart)
            {
                //TODO : save in DB cart item
            }
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
