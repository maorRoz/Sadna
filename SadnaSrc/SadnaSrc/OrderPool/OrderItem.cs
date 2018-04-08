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
        public string Store { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public OrderItem(string store, string name, double price, int quantity)
        {
            Store = store;
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        // TODO add price when there will be getters.
        // TODO might need to change the CartItem to Product once its implemented.
        public OrderItem(CartItem item)
        {
            Store = item.GetStore();
            Name = item.GetName();
            Quantity = item.GetQuantity();
            Price = 5;


        }




    }
}
