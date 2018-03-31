using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.OrderPool
{
    public class PurchaseHistory
    {
        public string User  { get; }
        public string Product { get; }
        public string Store { get; }
        public string Sale { get; }
        public string Date  { get; }
        public PurchaseHistory(string userName,string productName, string storeName, string saleType, string date)
        {
            User = userName;
            Product = productName;
            Store = storeName;
            Sale = saleType;
            Date = date;
        }
    }
}
