using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    public class RegisteredUser : User
    {
        private string name;
        private string address;
        private string password;

        private void RegisteredUserSetter(string name, string address, string password, CartItem[] savedCart)
        {
            this.name = name;
            this.address = address;
            this.password = password;
            cart.EnableCartSave();
            cart.LoadCart(savedCart);
        }
        public RegisteredUser(int systemID,string name,string address,string password,CartItem[] guestCart) 
            : base(systemID)
        {
            RegisteredUserSetter(name, address, password, guestCart);
            PolicyService.AddStatePolicy(UserPolicy.State.RegisteredUser);

        }

        public RegisteredUser(int loadedSystemID, string loadednName, string loadedAddress,
            string loadedPassword, CartItem[] loadedCart, UserPolicy[] loadedPolicies) 
            : base(loadedSystemID)
        {
            RegisteredUserSetter(loadednName, loadedAddress, loadedPassword, loadedCart);
            PolicyService.LoadPolicies(loadedPolicies);
        }

        public override object[] ToData()
        {
            object[] ret = { systemID, name, address, password };
            return ret;
        }

        public void PromoteToAdmin()
        {
            PolicyService.AddStatePolicy(UserPolicy.State.SystemAdmin);
        }
        public void AddUserPolicy(string store, List<StoreAdminPolicy.StoreAction> actionsToAdd)
        {
            PolicyService.UpdateStorePolicies(store,actionsToAdd);
        }
    }
}
