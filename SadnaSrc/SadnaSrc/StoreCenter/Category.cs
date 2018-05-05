using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    public class Category
    {
        public readonly string SystemId;
        string name { get; set; }
        string storeID;
        public LinkedList<Product> products { get; set; }
        private static int globalCategoryID = FindMaxCategoryID();

        public Category (string _Categoryname, string _storeID)
        {
            SystemId = getNextCategoryID();
            name = _Categoryname;
            storeID = _storeID;
            products = new LinkedList<Product>();
        }

        public Category (string _SystemId, string _name, string _storeID)
        {
            IStoreDL instacne = StoreDL.Instance;
            SystemId = _SystemId;
            name = _name;
            storeID = _storeID;
            products = instacne.GetAllCategoryProducts(SystemId);
        }
        public object[] GetCategoryValuesArray()
        {
            return new object[]
            {
                SystemId,
                name,
                storeID
            };
        }

        public string[] GetCategoryStringValues()
        {
            return new[]
            {
                "'" + SystemId + "'",
                "'" + name + "'",
                "'" + storeID + "'"
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
    }
}
