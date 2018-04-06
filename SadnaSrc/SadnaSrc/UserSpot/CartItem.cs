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
        public string Store { get; }
        public string Name { get; }

        public double UnitPrice { get; }
        public double FinalPrice => UnitPrice * Quantity;
        public string Sale { get; }
        public int Quantity { get; private set; }

        public CartItem( string name, string store, int quantity,double unitPrice, string sale)
        {
            Name = name;
            Store = store;
            Quantity = quantity;
            UnitPrice = unitPrice;
            Sale = sale;
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
            return new object[] {Name, Store, Quantity, FinalPrice, Sale};
        }

        public string GetDbIdentifier()
        {
            return " AND Name = '" + Name +
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

        public override int GetHashCode()
        {
            var hashCode = -776670310;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Store);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + UnitPrice.GetHashCode();
            hashCode = hashCode * -1521134295 + FinalPrice.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Sale);
            hashCode = hashCode * -1521134295 + Quantity.GetHashCode();
            return hashCode;
        }
    }
}
