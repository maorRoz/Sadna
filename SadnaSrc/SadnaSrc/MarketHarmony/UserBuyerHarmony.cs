using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using SadnaSrc.UserSpot;

namespace SadnaSrc.MarketHarmony
{
    class UserBuyerHarmony : IUserBuyer
    {
        private UserService _userService;
        public UserBuyerHarmony(ref IUserService userService)
        {
            _userService = (UserService) userService;
            //TODO: we should do something about user which hasn't entered the system here
        }

        public OrderItem[] Checkout()
        {
            CartItem[] userCart = _userService.CheckoutCart();
            //continue this shit and covert it into order items
            return null;
        }

        public string GetAddress()
        {
            return _userService.MarketUser.IsRegisteredUser() ? 
                ((RegisteredUser) _userService.MarketUser).Address : "";
        }

        public string GetName()
        {
            return _userService.MarketUser.IsRegisteredUser() ? 
                ((RegisteredUser)_userService.MarketUser).Name : "Guest";
        }
    }
}
