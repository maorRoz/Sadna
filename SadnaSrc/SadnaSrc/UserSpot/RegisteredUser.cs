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
        public string Name { get; private set; }
        public string Address { get; private set; }
        private string _password;
        public string CreditCard { get; private set; }

        private void InitiateRegisteredUser(string name, string address, string password, string creditCard, CartItem[] savedCart)
        {
            Name = name;
            Address = address;
            _password = password;
            CreditCard = creditCard;
            Cart.EnableCartSave();
            Cart.LoadCart(savedCart);
        }
        public RegisteredUser(IUserDL userDB,int systemID,string name,string address,string password, string creditCard,CartItem[] guestCart) 
            : base(userDB,systemID)
        {
            InitiateRegisteredUser(name, address, password, creditCard, guestCart);
            PolicyService.AddStatePolicy(StatePolicy.State.RegisteredUser);

        }

        public RegisteredUser(IUserDL userDB, int loadedSystemID, string loadednName, string loadedAddress,string loadedPassword,string loadedcreditCard,
            CartItem[] loadedCart, StatePolicy[] loadedStates, StoreManagerPolicy[] loadedStorePermissions) 
            : base(userDB,loadedSystemID)
        {
            InitiateRegisteredUser(loadednName, loadedAddress, loadedPassword, loadedcreditCard, loadedCart);
            PolicyService.LoadPolicies(loadedStates,loadedStorePermissions);
        }

        public override object[] ToData()
        {
            object[] ret = { SystemID, Name, Address, _password, CreditCard };
            return ret;
        }

        public void PromoteToAdmin()
        {
            PolicyService.AddStatePolicy(StatePolicy.State.SystemAdmin);
        }

        public void AddStoreOwnership(string store)
        {
            PolicyService.AddStoreOwnership(store);
        }
    }
}
