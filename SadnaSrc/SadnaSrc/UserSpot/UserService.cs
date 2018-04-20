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
        private int guestID;
        private int currentID;

        public UserService()
        {
            MarketUser = null;
        }

        public MarketAnswer EnterSystem()
        {
            EnterSystemSlave slave = new EnterSystemSlave();
            MarketUser = slave.EnterSystem();
            currentID = MarketUser.SystemID;
            guestID = MarketUser.SystemID;
            return slave.Answer;
        }

        public MarketAnswer SignUp(string name, string address, string password,string creditCard)
        {
            SignUpSlave slave = new SignUpSlave(MarketUser);
            MarketUser = slave.SignUp(name,address,password,creditCard);
            return slave.Answer;
        }

        public MarketAnswer SignIn(string name, string password)
        {
            SignInSlave slave = new SignInSlave(MarketUser);
            MarketUser = slave.SignIn(name,password);
            currentID = MarketUser.SystemID;
            return slave.Answer;
        }

        public MarketAnswer ViewCart()
        {
            ViewCartSlave slave = new ViewCartSlave(MarketUser);
            slave.ViewCart();
            return slave.Answer;

        }

        public MarketAnswer EditCartItem(string store, string product, double unitPrice, int quantity)
        {
            EditCartItemSlave slave = new EditCartItemSlave(MarketUser);
            slave.EditCartItem(store,product,unitPrice,quantity);
            return slave.Answer;

        }

        public MarketAnswer RemoveFromCart(string store, string product, double unitPrice)
        {
            RemoveFromCartSlave slave = new RemoveFromCartSlave(MarketUser);
            slave.RemoveFromCart(store,product,unitPrice);
            return slave.Answer;
        }

        public void AddToCart(string product, string store, int quantity, double unitPrice)
        {
            MarketUser.Cart.AddToCart(store,product,unitPrice,quantity);
        }


        public void CleanSession()
        {
            var userDB = UserServiceDL.Instance;
            userDB.DeleteUser(guestID);
            userDB.DeleteUser(currentID);
        }

    }
}
