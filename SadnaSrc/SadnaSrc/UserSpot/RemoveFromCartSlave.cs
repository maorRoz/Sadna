﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketData;

namespace SadnaSrc.UserSpot
{
    public class RemoveFromCartSlave
    {

        private readonly User _user;

        public UserAnswer Answer { get; private set; }

        private int userID;

        public RemoveFromCartSlave(User user)
        {
            Answer = null;
            _user = user;
            userID = user?.SystemID ?? -1;
        }
        public void RemoveFromCart(string product, string store, double unitPrice)
        {
            try
            {
                MarketLog.Log("UserSpot",
                    "User " + userID + " attempting to remove his cart item: " + product + " from store: " + store +
                    " ...");
                CartItem toRemove = ApproveModifyCart(store, product, unitPrice);

                MarketLog.Log("UserSpot",
                    "User " + userID + " found cart item: " + product + " from store: " + store +
                    ". proceeding for the removal...");
                _user.Cart.RemoveFromCart(toRemove);
                MarketLog.Log("UserSpot",
                    "User " + userID + "successfully removed cart item: " + product + " from store: " + store + " ...");
                Answer = new UserAnswer(RemoveFromCartStatus.Success, "Remove Cart Item has been successful!");
            }
            catch (UserException e)
            {
                Answer = new UserAnswer((RemoveFromCartStatus) e.Status, e.GetErrorMessage());
            }
            catch (DataException e)
            {
                Answer = new UserAnswer((RemoveFromCartStatus)e.Status, e.GetErrorMessage());
            }
        }

        private void ApproveEnetered()
        {
            if (_user != null) { return; }
            throw new UserException(RemoveFromCartStatus.DidntEnterSystem,
                "Remove Cart Item action has been requested by User which hasn't fully entered the system yet!");

        }


        private CartItem ApproveModifyCart(string store, string product, double unitPrice)
        {
            ApproveEnetered();
            CartItem found = _user.Cart.SearchInCart(store, product, unitPrice);
            if (found != null)
            {
                return found;
            }
            throw new UserException(RemoveFromCartStatus.NoItemFound,
                "Remove Cart Item operation found no item to modify!");

        }
    }
}
