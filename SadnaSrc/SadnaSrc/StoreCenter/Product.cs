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
        public List<string> Categories { get; set; }
        private static int globalProductID = -1;

        public Product(string _name, double _price, string _description)
        {
            SystemId = GetProductID();
            Name = _name;
            BasePrice = _price;
            Description = _description;
            Categories = new List<string>();
        }

        public Product(string _SystemId, string _name, double _price, string _description)
        {
            SystemId = _SystemId;
            Name = _name;
            BasePrice = _price;
            Description = _description;
	        Categories = new List<string>();

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
        
        private static string GetProductID()
        {
            if (globalProductID == -1)
            {
                globalProductID = StockSyncher.GetMaxEntityID(StoreDL.Instance.GetAllProductIDs());
            }
            globalProductID++;
            return "P" + globalProductID;
        }

        public static void RestartProductID()
        {
            globalProductID = -1;
        }

    }
}