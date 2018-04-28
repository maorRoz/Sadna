using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.StoreCenter
{
    public class StockListItem
    {
        public string SystemId { get; set; }
        public int Quantity { get; set; }
        public Product Product { get; set; }
        public Discount Discount { get; set; }

        public PurchaseEnum PurchaseWay { get; set; }

        public StockListItem(int quantity, Product product, Discount discount, PurchaseEnum purchaseWay,
            string id)
        {
            SystemId = id;
            Quantity = quantity;
            Product = product;
            Discount = discount;
            PurchaseWay = purchaseWay;
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
                return false;
            return Equals((StockListItem)obj);
        }
        private bool Equals(StockListItem obj)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            return (SystemId.Equals(obj.SystemId)) &&
                (Quantity == obj.Quantity) &&
                (Product.SystemId == obj.Product.SystemId) &&
                (Discount.discountCode == obj.Discount.discountCode) &&
                (EnumStringConverter.PrintEnum(PurchaseWay).Equals(EnumStringConverter.PrintEnum(obj.PurchaseWay)));
        }

        public override int GetHashCode()
        {
            var hashCode = -125935732;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SystemId);
            hashCode = hashCode * -1521134295 + Quantity.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Product>.Default.GetHashCode(Product);
            hashCode = hashCode * -1521134295 + EqualityComparer<Discount>.Default.GetHashCode(Discount);
            hashCode = hashCode * -1521134295 + PurchaseWay.GetHashCode();
            return hashCode;
        }
    }
}