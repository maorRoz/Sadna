using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace SadnaSrc.MarketHarmony
{
    class UserShopperHarmony : IUserShopper
    {
        private UserService _userService;
        public UserShopperHarmony(ref IUserService userService)
        {
            _userService = (UserService)userService;
        }

        public void ValidateCanBrowseMarket()
        {
            if (_userService.MarketUser == null)
            {
                throw new UserException(BrowseMarketStatus.DidntEnterSystem,"Cannot do actions in market without entering the system!");
            }
        }

        public void ValidateCanOpenStore()
        {
            ValidateCanBrowseMarket();
            if (!_userService.MarketUser.IsRegisteredUser())
            {
                throw new UserException(BrowseMarketStatus.DidntLoggedSystem, "Cannot open store as a guest!");
            }
        }


        public void AddToCart(Product product,string store,int quantity,string sale)
        {
            _userService.AddToCart(product.Name,store,quantity,product.BasePrice,sale);
        }

        public void AddOwnership(string store)
        {
            ((RegisteredUser) _userService.MarketUser).AddStoreOwnership(store);

        }

        //only for unit tests of StoreCenter shopping session(and not for Integration)
        public void LogInShopper(string userName, string password)
        {
            _userService.EnterSystem();
            _userService.SignIn(userName, password);
        }

        //only for unit tests of StoreCenter shopping session(and not for Integration)
        public void MakeGuest()
        {
            _userService.EnterSystem();
        }
    }
}
