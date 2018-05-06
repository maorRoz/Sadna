using System;
using System.Collections.Generic;

namespace SadnaSrc.StoreCenter
{
    public class Category
    {
        public readonly string SystemId;
        string Name { get; set; }
        public LinkedList<Product> products { get; set; }
        private static int globalCategoryID = FindMaxCategoryID();

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

        public string[] GetCategoryStringValues()
        {
            return new[]
            {
                "'" + SystemId + "'",
                "'" + Name + "'"
            };
        }
        public void addProductToCategory(Product product)
        {
            products.AddLast(product);
        }
        private static int FindMaxCategoryID()
        {
            StoreDL DL = StoreDL.Instance;
            LinkedList<string> list = DL.GetAllCategorysIDs();
            int max = -5;
            foreach (string s in list)
            {
                var temp = Int32.Parse(s.Substring(1));
                if (temp > max)
                {
                    max = temp;
                }
            }
            if (max < 0) return 1;
            return max;
        }
        private string getNextCategoryID()
        {

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
    }
}
