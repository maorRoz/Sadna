using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    public class CartService
    {
        private List<CartItem> cartStorage;
        private static UserServiceDL _userDL;
        private bool _toSave;

        public CartService()
        {
            cartStorage = new List<CartItem>();
            _toSave = false;
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

        public void LoadCart(CartItem[] loadedStorage)
        {
            foreach(CartItem item in loadedStorage)
            {
                cartStorage.Add(item);
            }
        }

        public void SaveCart(CartItem[] toSaveStorage)
        {
                _userDL.SaveCartItem(toSaveStorage);
        }
        public void AddToCart(string store,string product,double finalPrice,string sale,int quantity)
        {
            CartItem toAdd = new CartItem(store, product, finalPrice, sale, quantity);
            if (cartStorage.Contains(toAdd))
            {
                IncreaseCartItem(store,product);
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

        public void IncreaseCartItem(string store, string product)
        {
            CartItem found = searchInCart(store, product);
            found.IncreaseQuantity();
            cartStorage.Add(found);
            if (_toSave)
            {
                _userDL.UpdateCartItemQuantity(found.GetQuantity());
            }
        }

        public void DecreaseCartItem(string store, string product)
        {
            CartItem found = searchInCart(store, product);
            found.DecreaseQuantity();
            cartStorage.Add(found);
            if (_toSave)
            {
                _userDL.UpdateCartItemQuantity(found.GetQuantity());
            }
        }

        public void RemoveFromCart(string store,string product)
        {
            CartItem found = searchInCart(store, product);
            if (_toSave)
            {
                _userDL.RemoveCartItem(found);
            }
        }

        private CartItem searchInCart(string store, string product)
        {
            CartItem ret = null;
            foreach (CartItem item in cartStorage)
            {
                if (item.GetStore() == store && item.GetName() == product)
                {
                    ret = item;
                    break;
                }
            }
            if (ret == null)
            {
                throw new UserException("There is no cart item to be modified");
            }
            cartStorage.Remove(ret);
            return ret;
        }
    }
}
