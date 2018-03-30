using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    public class CartItem
    {
        private int _systemID;
        private string _store;
        private string _name;
        private double _finalPrice;
        private string _sale;
        private int _quantity;

        public CartItem(int systemID, string name, string store, int quantity, double finalPrice, string sale)
        {
            _systemID = systemID;
            _name = name;
            _store = store;
            _quantity = quantity;
            _finalPrice = finalPrice;
            _sale = sale;
        }

        public void SetUserID(int systemID)
        {
            _systemID = systemID;
        }

        public void IncreaseQuantity()
        {
            _quantity++;
        }

        public void DecreaseQuantity()
        {
            _quantity--;
        }

        public int GetQuantity()
        {
            return _quantity;
        }

        public string GetStore()
        {
            return _store;
        }

        public string GetName()
        {
            return _name;
        }

        public object[] ToData()
        {
            return new object[] { _systemID, _name, _store, _quantity, _finalPrice, _sale};
        }
    }
}
