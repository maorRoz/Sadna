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
        public string Name { get; }

        public string Store { get; }

        public int Quantity { get; set; }
        public double UnitPrice { get; }

        public double FinalPrice => UnitPrice * Quantity;

        public CartItem( string name, string store, int quantity,double unitPrice)
        {
            Name = name;
            Store = store;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public void ChangeQuantity(int quantity)
        {
            if (Quantity + quantity <= 0)
            {
                throw new UserException(EditCartItemStatus.ZeroNegativeQuantity, "Cannot hold quantity of zero or negative value in cart item");
            }
            Quantity += quantity;
           
        }

        public object[] ToData()
        {
            return new object[] {Name, Store, Quantity,UnitPrice, FinalPrice};
        }

        public string GetDbIdentifier()
        {
            return "Name = '" + Name +
                   "' AND Store = '" + Store +"' AND UnitPrice = "+ UnitPrice ;
        }

        public override string ToString()
        {
            return "Name : " + Name + " Store " + Store + " Quantity: " + Quantity + " Unit Price : " + UnitPrice
                   + " Final Price: " + FinalPrice ;
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
            return obj.Store.Equals(Store) && obj.Name.Equals(Name) && obj.UnitPrice == UnitPrice;
        }

        public bool Equals(string store, string name, double unitPrice)
        {
            return Store.Equals(store) && Name.Equals(name) && UnitPrice == unitPrice;
        }

        public override int GetHashCode()
        {
            var hashCode = -609274708;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Store);
            hashCode = hashCode * -1521134295 + Quantity.GetHashCode();
            hashCode = hashCode * -1521134295 + UnitPrice.GetHashCode();
            hashCode = hashCode * -1521134295 + FinalPrice.GetHashCode();
            return hashCode;
        }
    }
}
