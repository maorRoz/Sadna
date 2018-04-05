﻿using System;
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
        public UserBuyerHarmony(IUserService userService)
        {
            _userService = (UserService) userService;
        }

        public OrderItem[] Checkout()
        {
            CartItem[] userCart = _userService.CheckoutCart();
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
                ((RegisteredUser)_userService.MarketUser).Name : "";
        }
    }
}
