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
        public List<string> Categories { get; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public OrderItem(string store, List<string> categories, string name, double price, int quantity)
        {
            Store = store;
            Categories = categories;
            Name = name;
            Price = price;
            Quantity = quantity;
        }

       



    }
}
