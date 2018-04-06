using System.Collections.Generic;

namespace SadnaSrc.AdminView
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

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((PurchaseHistory) obj);
        }

        private bool Equals(PurchaseHistory obj)
        {
            return obj.User.Equals(User) && obj.Product.Equals(Product) && obj.Store.Equals(Store) 
                                         && obj.Sale.Equals(Sale) && obj.Date.Equals(Date);
        }

        public override string ToString()
        {
            return "User: " + User + "Product: " + Product + "Store: " + Store + "Sale: " + Sale + "Date: " + Date;
        }
        public override int GetHashCode()
        {
            var hashCode = -1637592205;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(User);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Product);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Store);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Sale);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Date);
            return hashCode;
        }
    }
}
