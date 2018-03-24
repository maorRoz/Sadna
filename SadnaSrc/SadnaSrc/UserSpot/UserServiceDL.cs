using System;
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
        public UserServiceDL(SQLiteConnection dbConnection,int systemID) : base(dbConnection)
        {
            _systemID = systemID;
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
