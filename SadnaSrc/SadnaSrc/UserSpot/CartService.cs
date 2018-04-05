﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    public class CartService
    {
        private List<CartItem> cartStorage;
        private static UserServiceDL _userDL;
        private bool _toSave;
        private int _systemID;

        public CartService(int systemID)
        {
            cartStorage = new List<CartItem>();
            _toSave = false;
            _systemID = systemID;
        }

        public void EnableCartSave()
        {
            _toSave = true;
        }
        public static void EstablishServiceDL(UserServiceDL userDL)
        {
            _userDL = userDL;
        }

        public CartItem[] GetCartStorage()
        {
            return cartStorage.ToArray();
        }
        public CartItem SearchInCart(string store, string product, double unitPrice, string sale)
        {

            foreach (CartItem item in cartStorage)
            {
                if (item.Equals(store,product,unitPrice,sale))
                {
                    return item;
                }
            }
            return null;
        }

        public void LoadCart(CartItem[] loadedStorage)
        {
            foreach(CartItem item in loadedStorage)
            {
                cartStorage.Add(item);
            }
        }

        public void EmptyCart()
        {
            cartStorage.Clear();
            _userDL.RemoveCart();
        }

        //TODO: Replace the arguments with product class
        public void AddToCart(string store,string product,double unitPrice,string sale,int quantity)
        {
            CartItem toAdd = new CartItem(_systemID, product,store, quantity, unitPrice, sale);
            if (cartStorage.Contains(toAdd))
            {
                EditCartItem(store,product,unitPrice,sale, quantity);
            }
            else
            {
                cartStorage.Add(toAdd);
                if (_toSave)
                {
                    _userDL.SaveCartItem(new []{toAdd});
                }
            }            
        }

        private void EditCartItem(string store, string product, double unitPrice, string sale, int quantity)
        {
            foreach (CartItem item in cartStorage)
            {
                if (!item.Equals(store, product, unitPrice, sale)) {continue;}
                item.ChangeQuantity(quantity);
                if (_toSave)
                {
                    _userDL.UpdateCartItemQuantity(item);
                }
            }
        }

        public void RemoveFromCart(string store,string product, double unitPrice, string sale)
        {
            foreach (CartItem item in cartStorage)
            {
                if (!item.Equals(store, product, unitPrice, sale)) { continue; }
                if (_toSave)
                {
                    _userDL.RemoveCartItem(item);
                }
            }
        }

    }
}
