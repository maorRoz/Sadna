using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    public class Product
    {
        public string SystemId;
        public string Name { get; set; }
        public int BasePrice { get; set; }
        public string Description { get; set; }

        public Product(string _SystemId, string _name, int _price, string _description)
        {
            SystemId = _SystemId;
            Name = _name;
            BasePrice = _price;
            Description = _description;
        }
        public object[] ToData()
        {
            return new object[] { SystemId, Name, BasePrice, Description };
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Product)obj);
        }


        private bool Equals(Product obj)
        {
            return ((SystemId == obj.SystemId)&(Name== obj.Name) & (BasePrice == obj.BasePrice));
        }
        public override string ToString()
        {
            return "SystemId: "+ SystemId +" name: " + Name+" base price: "+BasePrice+ " description: "+ Description;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (SystemId != null ? SystemId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ BasePrice;
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}

