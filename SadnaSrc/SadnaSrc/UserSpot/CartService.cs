using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    class CartService
    {
        private List<CartItem> cartStorage;
        private static UserServiceDL _userDL;

        public CartService()
        {
            cartStorage = new List<CartItem>();
        }
        public static void EstablishServiceDL(UserServiceDL userDL)
        {
            _userDL = userDL;
        }

        public CartItem[] GetCartStorage()
        {
            return cartStorage.ToArray();
        }

        public void AddToCart(string store,string product,double finalPrice,string sale,int quantity)
        {
            cartStorage.Add(new CartItem(store,product,finalPrice,sale,quantity));
        }
        public void IncreaseCartItem(string store, string product) { }
        public void DecreaseCartItem(string store, string product) { }

        public void RemoveFromCart(string store,string product)
        {

        }
    }
}
