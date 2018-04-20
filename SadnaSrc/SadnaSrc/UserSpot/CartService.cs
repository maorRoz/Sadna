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
        private int _userID;
        private List<CartItem> cartStorage;
        private UserServiceDL userDB;
        private bool _toSave;

        public CartService(int userID)
        {
            _userID = userID;
            userDB = UserServiceDL.Instance;
            cartStorage = new List<CartItem>();
            _toSave = false;
        }

        public void EnableCartSave()
        {
            _toSave = true;
        }

        private static CartItem[] SortedCartStorage(CartItem[] storage)
        {
            return storage.OrderBy(x => x.Quantity).ToArray();
        }

        public CartItem[] GetCartStorage()
        {
            return SortedCartStorage(cartStorage.ToArray());
        }


        public CartItem[] GetCartStorage(string store)
        {
            List<CartItem> filteredCartStorage = new List<CartItem>();
            foreach (CartItem item in cartStorage)
            {
                if (item.Store.Equals(store))
                {
                    filteredCartStorage.Add(item);
                }
            }

            return SortedCartStorage(filteredCartStorage.ToArray());
        }
        public CartItem SearchInCart(string store, string product, double unitPrice)
        {

            foreach (CartItem item in cartStorage)
            {
                if (item.Equals(store,product,unitPrice))
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
            userDB.RemoveCart(_userID);
        }

        public void EmptyCart(string store)
        {
            List<CartItem> filteredStorage = new List<CartItem>();
            foreach (CartItem item in cartStorage)
            {
                if (_toSave && item.Store.Equals(store))
                {
                    userDB.RemoveCartItem(_userID, item);
                }
                else
                {
                    filteredStorage.Add(item);
                }
            }
            cartStorage = filteredStorage;
        }

        public void AddToCart(string store,string product,double unitPrice,int quantity)
        {
            CartItem toAdd = new CartItem(product,store, quantity, unitPrice);
            if (cartStorage.Contains(toAdd))
            {
                EditCartItem(toAdd, quantity);
            }
            else
            {
                cartStorage.Add(toAdd);
                if (_toSave)
                {
                    userDB.SaveCartItem(_userID,new[]{toAdd});
                }
            }            
        }

        public void EditCartItem(CartItem toEdit, int quantity)
        {
            foreach (CartItem item in cartStorage)
            {
                if (!item.Equals(toEdit)) {continue;}
                item.ChangeQuantity(quantity);
                if (_toSave)
                {
                    userDB.UpdateCartItemQuantity(item);
                }
            }
        }

        public void RemoveFromCart(CartItem toRemove)
        {
            foreach (CartItem item in cartStorage)
            {
                if (!item.Equals(toRemove)) { continue; }

                cartStorage.Remove(item);
                if (_toSave)
                {
                    userDB.RemoveCartItem(_userID,item);
                }
                break;
            }
        }

    }
}
