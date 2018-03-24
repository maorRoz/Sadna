using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    class CartItem
    {
        private string _store;
        private string _name;
        private double _finalPrice;
        private string _sale;
        private int _quantity;

        public CartItem(string store, string name, double finalPrice, string sale, int quantity)
        {
            _store = store;
            _name = name;
            _finalPrice = finalPrice;
            _sale = sale;
            _quantity = quantity;
        }

        public void IncreaseQuantity()
        {
            _quantity++;
        }

        public void DecreaseQuantity()
        {
            _quantity--;
        }
    }
}
