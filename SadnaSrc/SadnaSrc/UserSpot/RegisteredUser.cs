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

        private void InitiateRegisteredUser(string name, string address, string password, CartItem[] savedCart)
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
            InitiateRegisteredUser(name, address, password, guestCart);
            PolicyService.AddStatePolicy(StatePolicy.State.RegisteredUser);

        }

        public RegisteredUser(int loadedSystemID, string loadednName, string loadedAddress,
            string loadedPassword, CartItem[] loadedCart, StatePolicy[] loadedStates, StoreManagerPolicy[] loadedStorePermissions) 
            : base(loadedSystemID)
        {
            InitiateRegisteredUser(loadednName, loadedAddress, loadedPassword, loadedCart);
            PolicyService.LoadPolicies(loadedStates,loadedStorePermissions);
        }

        public override object[] ToData()
        {
            object[] ret = { systemID, name, address, password };
            return ret;
        }

        public void PromoteToAdmin()
        {
            PolicyService.AddStatePolicy(StatePolicy.State.SystemAdmin);
        }
        public void AddStoreManagerPolicy(string store, StoreManagerPolicy.StoreAction[] actionsToAdd)
        {
            PolicyService.UpdateStorePolicies(store,actionsToAdd);
        }
    }
}
