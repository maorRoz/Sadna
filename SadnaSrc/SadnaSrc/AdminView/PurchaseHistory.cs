using System.Collections.Generic;
using SadnaSrc.StoreCenter;

namespace SadnaSrc.AdminView
{
    public class PurchaseHistory
    {
        private string _user;
        private string _product;
        private string _store;
        private string _sale;
        private int _quantity;
        private double _price;
        private string _date;

        public PurchaseHistory(string userName,string productName, string storeName, string saleType, int quantity, double price, string date)
        {
            _user = userName;
            _product = productName;
            _store = storeName;
            _sale = saleType;
            _quantity = quantity;
            _price = price;
            _date = date;
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
            return obj._user.Equals(_user) && obj._product.Equals(_product) && obj._store.Equals(_store) 
                          && obj._sale.Equals(_sale) && obj._quantity == _quantity && obj._price == _price && obj._date.Equals(_date);
        }

        public override string ToString()
        {
            return "User: " + _user + "Product: " + _product + "Store: " + _store + "Sale: " + _sale 
                   + "Quantity: "+ _quantity + "Price: " + _price + "Date: " + _date;
        }
        public override int GetHashCode()
        {
            var hashCode = -1637592205;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(_user);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(_product);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(_store);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(_sale);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(_date);
            return hashCode;
        }
    }
}
