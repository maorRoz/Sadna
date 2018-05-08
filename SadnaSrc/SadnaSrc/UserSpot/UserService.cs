using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketFeed;
using SadnaSrc.StoreCenter;

namespace SadnaSrc.UserSpot
{
    public class UserService : IUserService
    {
        public User MarketUser { get; private set; }
        private UserDL userDB;

        public UserService()
        {
            MarketUser = null;
            userDB = UserDL.Instance;
        }

        public MarketAnswer EnterSystem()
        {
            EnterSystemSlave slave = new EnterSystemSlave(userDB);
            MarketUser = slave.EnterSystem();
            return slave.Answer;
        }

        public MarketAnswer SignUp(string name, string address, string password,string creditCard)
        {
            SignUpSlave slave = new SignUpSlave(MarketUser,userDB,Publisher.Instance);
            MarketUser = slave.SignUp(name,address,password,creditCard);
            return slave.Answer;
        }

        public MarketAnswer SignIn(string name, string password)
        {
            SignInSlave slave = new SignInSlave(MarketUser, userDB);
            MarketUser = slave.SignIn(name,password);
            return slave.Answer;
        }

        public MarketAnswer ViewCart()
        {
            ViewCartSlave slave = new ViewCartSlave(MarketUser);
            slave.ViewCart();
            return slave.Answer;

        }

		public MarketAnswer ViewUsers()
		{
			ViewUsersSlave slave = new ViewUsersSlave(MarketUser,userDB);
			slave.ViewUsers();
			return slave.Answer;
		}

        public MarketAnswer EditCartItem(string store, string product, int quantity, double unitPrice)
        {
            EditCartItemSlave slave = new EditCartItemSlave(MarketUser);
            slave.EditCartItem(store,product,quantity, unitPrice);
            return slave.Answer;

        }

        public MarketAnswer RemoveFromCart(string store, string product, double unitPrice)
        {
            RemoveFromCartSlave slave = new RemoveFromCartSlave(MarketUser);
            slave.RemoveFromCart(product, store, unitPrice);
            return slave.Answer;
        }

        public MarketAnswer GetControlledStoreNames()
        {
            GetControlledStoreNamesSlave slave = new GetControlledStoreNamesSlave(MarketUser,userDB);
            slave.GetControlledStoreNames();
            return slave.Answer;
        }
		

        public MarketAnswer GetStoreManagerPolicies(string store)
        {
          GetStoreManagerPoliciesSlave slave = new GetStoreManagerPoliciesSlave(MarketUser);
          slave.GetStoreManagerPolicies(store);
          return slave.Answer;

        }
        public MarketAnswer GetUserDetails()
        {
            GetUserDetailsSlave slave = new GetUserDetailsSlave(MarketUser);
            slave.GetUserDetails();
            return slave.Answer;
        }

	    public MarketAnswer GetAllStores()
	    {
		    GetControlledStoreNamesSlave slave = new GetControlledStoreNamesSlave(MarketUser,userDB);
			slave.ViewStores();
			return slave.Answer;
		}

	    public void AddToCart(string product, string store, int quantity, double unitPrice)
        {
            MarketUser.Cart.AddToCart(product, store, quantity,unitPrice);
        }
	}
}
