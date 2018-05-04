using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    public class Product
    {
        public readonly string SystemId;
        public string Name { get; set; }
        public double BasePrice { get; set; }
        public string Description { get; set; }
        private static int globalProductID = FindMaxProductId();

        public Product(string _name, double _price, string _description)
        {
            SystemId = GetProductID();
            Name = _name;
            BasePrice = _price;
            Description = _description;
        }

        public Product(string _SystemId, string _name, double _price, string _description)
        {
            SystemId = _SystemId;
            Name = _name;
            BasePrice = _price;
            Description = _description;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.GetType() == GetType() && Equals((Product)obj);
        }


        private bool Equals(Product obj)
        {
            return ((SystemId == obj.SystemId) & (Name == obj.Name) & (BasePrice == obj.BasePrice));
        }
        public override string ToString()
        {
            return " name: " + Name + " base price: " + BasePrice + " description: " + Description;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (SystemId != null ? SystemId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                return hashCode;
            }
        }
        public object[] GetProductValuesArray()
        {
            return new object[]
            {
                SystemId,
                Name,
                BasePrice,
                Description
            };
        }

        public string[] GetProductStringValues()
        {
            return new[]
            {
                "'" + SystemId + "'",
                "'" + Name + "'",
                BasePrice + "",
                "'" + Description + "'"
            };
        }
        
        private static string GetProductID()
        {
            globalProductID++;
            return "P" + globalProductID;
        }

        private static int FindMaxProductId()
        {
            StoreDL DL = StoreDL.GetInstance();
            LinkedList<string> list = DL.GetAllProductIDs();
            int max = -5;
            int temp = 0;
            foreach (string s in list)
            {
                temp = Int32.Parse(s.Substring(1));
                if (temp > max)
                {
                    max = temp;
                }
            }
            return max;
        }

    }
}