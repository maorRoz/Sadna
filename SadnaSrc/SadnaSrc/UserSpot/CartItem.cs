using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    public class CartItem
    {
        private int _systemID;
        public string Store { get; }
        public string Name { get; }

        public double UnitPrice { get; }
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

        public void ChangeQuantity(int quantity)
        {
            if (Quantity + quantity <= 0)
            {
                throw new UserException(MarketError.LogicError, "Cannot hold quantity of zero or negative value in cart item");
            }
            Quantity += quantity;
           
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

        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((CartItem)obj);
        }


        private bool Equals(CartItem obj)
        {
            return obj.Store.Equals(Store) && obj.Name.Equals(Name) && obj.UnitPrice == UnitPrice &&
                   obj.Sale.Equals(Sale);
        }

        public bool Equals(string store, string name, double unitPrice, string sale)
        {
            return Store.Equals(store) && Name.Equals(name) && UnitPrice == unitPrice && Sale.Equals(sale);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _systemID;
                hashCode = (hashCode * 397) ^ (Store != null ? Store.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ UnitPrice.GetHashCode();
                hashCode = (hashCode * 397) ^ (Sale != null ? Sale.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Quantity;
                return hashCode;
            }
        }
        //TODO: delete this when there is no more reference related to it
        public string GetStore()
        {
            throw new NotImplementedException("dont use this method Igor!!");
        }

        //TODO: delete this when there is no more reference related to it
        public string GetName()
        {
            throw new NotImplementedException("dont use this method Igor!!");
        }

        //TODO: delete this when there is no more reference related to it
        public int GetQuantity()
        {
            throw new NotImplementedException("dont use this method Igor!!");
        }

    }
}
