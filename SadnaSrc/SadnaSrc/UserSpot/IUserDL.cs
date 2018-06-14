using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;

namespace SadnaSrc.UserSpot
{
    public interface IUserDL
    {
        int[] GetAllSystemIDs();
        void SaveUser(User user);
        RegisteredUser RegisterUser(int systemID, string name, string address, string encryptedPassword, string creditCard, CartItem[] cartItem);
        bool IsUserNameExist(string userName);
        RegisteredUser LoadUser(object[] userData, CartItem[] getCartStorage);
        string[] UserNamesInSystem();
        object [] FindRegisteredUserData(string userName, string password);
        void RemoveCart(int userId);
        void RemoveCartItem(int userId, CartItem item);
        void SaveCartItem(int userId, CartItem[] cartItems);
        void UpdateCartItemQuantity(CartItem item);
        void SaveUserStatePolicy(int userId, StatePolicy statePolicy);
        void SaveUserStorePolicy(int userId, StoreManagerPolicy storeOwnershipPermission);
	    string[] GetAllActiveStoreNames();
		void InsertSignedInUser(int systemId);
	    
    }
}
