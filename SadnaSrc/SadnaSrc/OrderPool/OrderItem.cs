using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.UserSpot;

namespace SadnaSrc.OrderPool
{
    public class OrderItem
    {
        private string _store;
        private string _name;
        private double _price;
        private int _quantity;

        public OrderItem(string store, string name, double price, int quantity)
        {
            _store = store;
            _name = name;
            _price = price;
            _quantity = quantity;
        }

        // TODO add price when there will be getters.
        // TODO might need to change the CartItem to Product once its implemented.
        public OrderItem(CartItem item)
        {
            _store = item.GetStore();
            _name = item.GetName();
            _quantity = item.GetQuantity();
            _price = 5;


        }

        public string GetStore() { return _store; }
        public string GetName() { return _name; }
        public double GetPrice() { return _price; }
        public int GetQuantity() { return _quantity; }



    }
}
