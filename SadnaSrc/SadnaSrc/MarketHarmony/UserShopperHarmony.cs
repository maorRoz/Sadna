using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.UserSpot;

namespace SadnaSrc.MarketHarmony
{
    class UserShopperHarmony : IUserShopper
    {
        private UserService _userService;
        public UserShopperHarmony(ref UserService userService)
        {
            _userService = userService;
            //TODO: we should do something about user which hasn't entered the system here
        }

        public void AddToCart()
        {
           // _userService.AddToCart(/* Product */);
        }

        public bool AddOwnership(string store)
        {
            if (_userService.MarketUser.IsRegisteredUser())
            {
                ((RegisteredUser) _userService.MarketUser).AddStoreOwnership(store);
                return true;
            }

            return false;
        }
    }
}
