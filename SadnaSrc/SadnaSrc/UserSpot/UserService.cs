using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using static System.Int32;

namespace SadnaSrc.UserSpot
{
    public class UserService : IUserService
    {
        public User MarketUser { get; private set; }
        private readonly UserServiceDL userDL;
        private int systemID;
        private int oldID;

        public UserService()
        {
            userDL = new UserServiceDL();
            MarketUser = null;
            systemID = userDL.GetSystemID();
            oldID = systemID;
            Synch();
        }

        public MarketAnswer EnterSystem()
        {
            MarketLog.Log("UserSpot", "User " + systemID + " attempting to enter the system...");
            MarketUser = new User(systemID);
            MarketLog.Log("UserSpot","User "+systemID+" has entered the system!");
            return new UserAnswer(EnterSystemStatus.Success,"You've been entered the system successfully!");
        }

        public MarketAnswer SignUp(string name, string address, string password,string creditCard)
        {
            MarketLog.Log("UserSpot", "User " + systemID + " attempting to sign up to the system...");
            try
            {
                ApproveSignUp(name, address, password,creditCard);
                string encryptedPassword = ToEncryptPassword(password);
                MarketLog.Log("UserSpot", "Searching for existing user and storing newly Registered User " + systemID + " data...");
                MarketUser = userDL.RegisterUser(name, address, encryptedPassword, creditCard, MarketUser.Cart.GetCartStorage());
                MarketLog.Log("UserSpot", "User " + systemID + " sign up to the system has been successfull!");
                return new UserAnswer(SignInStatus.Success, "Sign up has been successfull!");
            }
            catch (UserException e)
            {
                MarketLog.Log("UserSpot", "User " + systemID + " has failed to sign up. Error message has been created!");
                return new UserAnswer((SignUpStatus)e.Status, e.GetErrorMessage());
            }
        }

        public MarketAnswer SignIn(string name, string password)
        {
            MarketLog.Log("UserSpot", "User " + systemID + " attempting to sign in the system...");
            try
            {
                ApproveSignIn(name, password);
                string encryptedPassword = ToEncryptPassword(password);
                MarketLog.Log("UserSpot", "Searching for existing user and logging in Guest "
                                          + systemID + " into the system...");
                MarketUser = userDL.LoadUser(name, encryptedPassword, MarketUser.Cart.GetCartStorage());
                systemID = MarketUser.SystemID;
                MarketLog.Log("UserSpot", "User " + oldID + " sign in to the system has been successfull!");
                MarketLog.Log("UserSpot", "User " + oldID + " is now recognized as Registered User " + systemID);
                return new UserAnswer(SignInStatus.Success, "Sign in has been successful!");

            }
            catch (UserException e)
            {
                MarketLog.Log("UserSpot", "User " + systemID + " has failed to sign in. Error message has been created!");
                return new UserAnswer((SignInStatus)e.Status, e.GetErrorMessage());
            }
        }

        public MarketAnswer ViewCart()
        {
            MarketLog.Log("UserSpot", "User " + systemID + " attempting to view his cart...");
            try
            {
                ApproveEnetered("ViewCart");
                var itemRecords = new List<string>();
                foreach (CartItem item in MarketUser.Cart.GetCartStorage())
                {
                    itemRecords.Add(item.ToString());
                }

                MarketLog.Log("UserSpot", "User " + systemID + " has successfully retrieved his cart info...");
                return new UserAnswer(ViewCartStatus.Success, "View of the user's cart has been granted successfully!",
                    itemRecords.ToArray());
            }
            catch (UserException e)
            {
                MarketLog.Log("UserSpot", "User " + systemID + " has failed to View Cart. Error message has been created!");
                return new UserAnswer((ViewCartStatus) e.Status, e.GetErrorMessage(),null);
            }

        }

        public MarketAnswer EditCartItem(string store, string product, double unitPrice, int quantity)
        {
            MarketLog.Log("UserSpot", "User " + systemID + " attempting to edit his cart item: " +product + " from store: "+ store + " ...");
            try
            {
                CartItem toEdit = ApproveModifyCart("EditCartItem", store, product, unitPrice);

                MarketLog.Log("UserSpot","User " + systemID + " found cart item: " + product + " from store: " + store +". proceeding for the edit...");
                MarketUser.Cart.EditCartItem(toEdit, quantity);
                MarketLog.Log("UserSpot","User " + systemID + "successfully edited cart item: " + product + " from store: " + store +" ...");
                return new UserAnswer(EditCartItemStatus.Success, "Edit Cart Item has been successful!");
            }
            catch (UserException e)
            {
                MarketLog.Log("UserSpot","User " + systemID + " has failed to Edit Cart Item. Error message has been created!");
                return new UserAnswer((EditCartItemStatus) e.Status, e.GetErrorMessage());
            }

        }

        public MarketAnswer RemoveFromCart(string store, string product, double unitPrice)
        {
            MarketLog.Log("UserSpot", "User " + systemID + " attempting to remove his cart item: " + product + " from store: " + store + " ...");
            try
            {
                CartItem toRemove = ApproveModifyCart("RemoveFromCart", store, product, unitPrice);

                MarketLog.Log("UserSpot","User " + systemID + " found cart item: " + product + " from store: " + store +". proceeding for the removal...");
                MarketUser.Cart.RemoveFromCart(toRemove);
                MarketLog.Log("UserSpot","User " + systemID + "successfully removed cart item: " + product + " from store: " + store +" ...");
                return new UserAnswer(RemoveFromCartStatus.Success, "Remove Cart Item has been successful!");
            }
            catch (UserException e)
            {
                MarketLog.Log("UserSpot",
                    "User " + systemID + " has failed to Edit Cart Item. Error message has been created!");
                return new UserAnswer((RemoveFromCartStatus) e.Status, e.GetErrorMessage());
            }
        }

        private void ApproveEnetered(string action)
        {
            if (MarketUser != null) {return;}
            if (action.Equals("sign up"))
            {
                throw new UserException(SignUpStatus.DidntEnterSystem,
                    "sign up action has been requested by User which hasn't fully entered the system yet!");
            }
            if (action.Equals("sign in"))
            {
                throw new UserException(SignInStatus.DidntEnterSystem,
                    "sign in action has been requested by User which hasn't fully entered the system yet!");
            }

            if (action.Equals("ViewCart"))
            {
                throw new UserException(ViewCartStatus.DidntEnterSystem,
                    "View Cart action has been requested by User which hasn't fully entered the system yet!");
            }
            if (action.Equals("EditCartItem"))
            {
                throw new UserException(EditCartItemStatus.DidntEnterSystem,
                    "Edit Cart Item action has been requested by User which hasn't fully entered the system yet!");
            }
            throw new UserException(RemoveFromCartStatus.DidntEnterSystem,
                "Remove Cart Item action has been requested by User which hasn't fully entered the system yet!");

        }

        private void ApproveGuest(string action)
        {
            ApproveEnetered(action);
            if (!MarketUser.IsRegisteredUser())
            {
                return;
            }
            if (action.Equals("sign up"))
            {
                throw new UserException(SignUpStatus.SignedUpAlready,
                    "sign up action has been requested by registered user!");
            }
            throw new UserException(SignInStatus.SignedInAlready,
                "sign in action has been requested by registered user!");
        }

        private bool IsValidCreditCard(string creditCard)
        {
            return !string.IsNullOrEmpty(creditCard) && creditCard.Length == 8 && TryParse(creditCard,out _);
        }

        private void ApproveSignUp(string name, string address, string password, string creditCard)
        {
            ApproveGuest("sign up");
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(password) || !IsValidCreditCard(creditCard))
            {
                throw new UserException(SignUpStatus.NullEmptyFewDataGiven,
                    "sign up action has been requested while some required fields are still missing or invalid!");
            }
        }

        private CartItem ApproveModifyCart(string action, string store, string product, double unitPrice)
        {
            ApproveEnetered(action);
            CartItem found = MarketUser.Cart.SearchInCart(store, product, unitPrice);
            if (found != null)
            {
                return found;
            }
            if (action.Equals("EditCartItem"))
            {
                throw new UserException(EditCartItemStatus.NoItemFound,
                    "Edit Cart Item operation found no item to modify!");
            }
            throw new UserException(RemoveFromCartStatus.NoItemFound,
                "Remove Cart Item operation found no item to modify!");

        }
        private string ToEncryptPassword(string password)
        {
            MarketLog.Log("UserSpot", "encrypting User " + systemID + " password for security measures...");
            string encryptedPassword = GetSecuredPassword(password);
            MarketLog.Log("UserSpot", "User " + systemID + " password has been encrypted successfully!");
            return encryptedPassword;
        }

        public static string GetSecuredPassword(string password)
        {
            var secuirtyService = System.Security.Cryptography.MD5.Create();
            byte[] bytes = Encoding.Default.GetBytes(password);
            byte[] encodedBytes = secuirtyService.ComputeHash(bytes);

            StringBuilder newPasswordString = new StringBuilder();
            for (int i = 0; i < encodedBytes.Length; i++)
                newPasswordString.Append(encodedBytes[i].ToString("x2"));

            return newPasswordString.ToString();
        }

        private void ApproveSignIn(string name, string password)
        {
            ApproveGuest("sign in");
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
            {
                throw new UserException(SignInStatus.NullEmptyDataGiven,
                    "sign in action has been requested while some required fields are still missing!");
            }
        }

        public CartItem[] CheckoutCart()
        {
            CartItem[] storage = MarketUser.Cart.GetCartStorage();
            MarketUser.Cart.EmptyCart();
            return storage;
        }

        public CartItem[] CheckoutCartFromStore(string store)
        {
            CartItem[] storageFromStore = MarketUser.Cart.GetCartStorage(store);
            MarketUser.Cart.EmptyCart(store);
            return storageFromStore;
        }

        public CartItem CheckoutItem(string itemName, string store, int quantity, double unitPrice)
        {
            CartItem itemFromStore = MarketUser.Cart.SearchInCart(store,itemName,unitPrice);
            if (itemFromStore == null || quantity > itemFromStore.Quantity)
            {
                throw new UserException(EditCartItemStatus.NoItemFound,
                    "No item by that info or quantity has been found in the user cart!");
            }
            if(quantity == itemFromStore.Quantity)
            {
                MarketUser.Cart.RemoveFromCart(itemFromStore);
            }
            else
            {
                MarketUser.Cart.EditCartItem(itemFromStore,-quantity);
            }
            itemFromStore.ChangeQuantity(-quantity);
            return itemFromStore;
        }

        public void AddToCart(string product, string store, int quantity, double unitPrice)
        {
            MarketUser.Cart.AddToCart(store,product,unitPrice,quantity);
        }

        public void Synch()
        {
            UserException.SetUser(systemID);
            UserPolicyService.EstablishServiceDL(userDL);
            CartService.EstablishServiceDL(userDL);
        }
        public void CleanGuestSession()
        {
            Synch();
            userDL.DeleteUser(oldID);
        }

        public void CleanSession()
        {
            CleanGuestSession();
            userDL.DeleteUser(systemID);
        }

    }
}
