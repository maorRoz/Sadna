using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    class Product
    {
        private int SystemId;
        private String name { get; private set; }
        private int BasePrice { get; private set; }
        private String description { get; private set; }

        public Product(int _SystemId, String _name, int _price, String _description)
        {
            SystemId = _SystemId;
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
    }
}

