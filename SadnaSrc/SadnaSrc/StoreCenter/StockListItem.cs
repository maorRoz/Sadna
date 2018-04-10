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
        private bool Equals (StockListItem obj)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            return (SystemId.Equals(obj.SystemId)) &&
                (Quantity == obj.Quantity) &&
                (Product.SystemId == obj.Product.SystemId) &&
                (Discount.discountCode == obj.Discount.discountCode) &&
                (handler.PrintEnum(PurchaseWay).Equals(handler.PrintEnum(obj.PurchaseWay)));
        }
    }
}
