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
        public string Store { get; }
        public string Name { get; }

        private double UnitPrice { get; }
        public double FinalPrice => UnitPrice * Quantity;
        public string Sale { get; }
        public int Quantity { get; private set; }

        public CartItem(int systemID, string name, string store, int quantity,double unitPrice, string sale)
        {
            _systemID = systemID;
            Name = name;
            Store = store;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Sale = sale;
        }

        public void SetUserID(int systemID)
        {
            _systemID = systemID;
        }

        public void IncreaseQuantity()
        {
            Quantity++;
        }

        public void DecreaseQuantity()
        {
            Quantity--;
        }

        public object[] ToData()
        {
            return new object[] { _systemID, Name, Store, Quantity, FinalPrice, Sale};
        }

        public string GetDbIdentifier()
        {
            return "SystemID = " + _systemID + " AND Name = '" + Name +
                   "' AND Store = '" + Store +"' AND UnitPrice = "+ UnitPrice + " AND SaleType = '" + Sale + "'";
        }

        public string GetStore()
        {
            throw new NotImplementedException("dont use this method Igor!!");
        }

        public string GetName()
        {
            throw new NotImplementedException("dont use this method Igor!!");
        }

        public int GetQuantity()
        {
            throw new NotImplementedException("dont use this method Igor!!");
        }

    }
}
