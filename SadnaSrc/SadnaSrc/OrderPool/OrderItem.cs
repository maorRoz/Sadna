using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

       



    }
}
