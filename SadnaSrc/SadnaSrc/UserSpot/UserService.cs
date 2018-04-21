using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

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
            SignUpSlave slave = new SignUpSlave(MarketUser,userDB);
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

        public void AddToCart(string product, string store, int quantity, double unitPrice)
        {
            MarketUser.Cart.AddToCart(product, store, quantity,unitPrice);
        }


        public void CleanSession()
        {
            userDB.CleanSession();
        }

    }
}
