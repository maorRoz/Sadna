﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    public class User
    {
        protected readonly UserPolicyService PolicyService;
        protected int systemID;
        protected readonly CartService cart;

        public CartService Cart
        {
            get { return cart; }
        }

        public int SystemID
        {
            get { return systemID; }
        }
        public User(int systemID)
        {
            PolicyService = new UserPolicyService();
            cart = new CartService();
            this.systemID = systemID;
        }

        public UserPolicy[] GetPolicies()
        {
            return PolicyService.Policies.ToArray();
        }

        public CartItem[] GetCart()
        {
            return cart.GetCartStorage();
        }
        public virtual object[] ToData()
        {
            object[] ret = {systemID, null, null, null};
            return ret;
        }
    }
}
