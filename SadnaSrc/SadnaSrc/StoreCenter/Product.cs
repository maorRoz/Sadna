using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    public class Product
    {
        public String SystemId;
        public String name { get; set; }
        public int BasePrice { get; set; }
        public String description { get; set; }

        public Product(int _SystemId, String _name, int _price, String _description)
        {
            SystemId = "P"+_SystemId;
            name = _name;
            BasePrice = _price;
            description = _description;
        }
        public object[] ToData()
        {
            return new object[] { SystemId, name, BasePrice, description };
        }
        public bool equal(Product other)
        {
            return ((SystemId == other.SystemId)&(name==other.name) & (BasePrice == other.BasePrice));
        }
        public String toString()
        {
            return "name "+name+" base price: "+BasePrice;
        }
    }
}

