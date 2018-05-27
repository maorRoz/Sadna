using System;
using System.Collections.Generic;
using SadnaSrc.StoreCenter;

namespace SadnaSrc.MarketHarmony
{
    public class Category
    {
        public readonly string SystemId;
        public string Name { get; set; }
        public LinkedList<Product> products { get;}
        private static int globalCategoryID = -1;

        public Category (string categoryname)
        {
            SystemId = getNextCategoryID();
            Name = categoryname;
            products = new LinkedList<Product>();
        }

        public Category (string systemid, string name)
        {
            IStoreDL instacne = StoreDL.Instance;
            SystemId = systemid;
            Name = name;
            products = instacne.GetAllCategoryProducts(SystemId);
        }
        public object[] GetCategoryValuesArray()
        {
            return new object[]
            {
                SystemId,
                Name,
            };
        }
        public void addProductToCategory(Product product)
        {
            products.AddLast(product);
        }
        private string getNextCategoryID()
        {
            if (globalCategoryID == -1)
            {
                globalCategoryID = StockSyncher.GetMaxEntityID(StoreDL.Instance.GetAllCategorysIDs());
            }
            globalCategoryID++;
            return "C" + globalCategoryID;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.GetType() == GetType() && Equals((Category)obj);
        }
        private bool Equals(Category obj)
        {
            return obj.SystemId == SystemId && obj.Name == Name;
        }

        public override int GetHashCode()
        {
            var hashCode = -1305757020;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SystemId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<LinkedList<Product>>.Default.GetHashCode(products);
            return hashCode;
        }
    }
}
