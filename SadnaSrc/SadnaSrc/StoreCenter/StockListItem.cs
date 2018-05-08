﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.StoreCenter
{
    public class StockListItem
    {
        private string systemId;
        public int Quantity { get; set; }
        public Product Product { get;}
        public Discount Discount { get; set; }

        public PurchaseEnum PurchaseWay { get; set; }

        public StockListItem(int quantity, Product product, Discount discount, PurchaseEnum purchaseWay,
            string id)
        {
            systemId = id;
            Quantity = quantity;
            Product = product;
            Discount = discount;
            PurchaseWay = purchaseWay;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.GetType() == GetType() && Equals((StockListItem)obj);
        }
        private bool Equals(StockListItem obj)
        {
            return systemId.Equals(obj.systemId) &&
                Quantity == obj.Quantity &&
                Product.SystemId == obj.Product.SystemId &&
                Discount.discountCode == obj.Discount.discountCode &&
                EnumStringConverter.PrintEnum(PurchaseWay).Equals(EnumStringConverter.PrintEnum(obj.PurchaseWay));
        }

        public override int GetHashCode()
        {
            var hashCode = -125935732;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(systemId);
            hashCode = hashCode * -1521134295 + Quantity.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Product>.Default.GetHashCode(Product);
            hashCode = hashCode * -1521134295 + EqualityComparer<Discount>.Default.GetHashCode(Discount);
            hashCode = hashCode * -1521134295 + PurchaseWay.GetHashCode();
            return hashCode;
        }
        public object[] GetStockListItemArray()
        {
            object discountObject = "";
            if (Discount != null)
            {
                discountObject = Discount;
            }

            return new []
            {
                systemId,
                Product,
                Quantity,
                discountObject,
                PurchaseWay
            };
        }
        public string[] GetStockListItemStringValues()
        {
            string ifDiscountNotExists = "null";
            if (Discount != null)
            {
                ifDiscountNotExists = Discount.discountCode;
            }

            return new[]
            {
                "'" + systemId + "'",
                "'" + Product.SystemId + "'",
                "'" + Quantity + "'",
                "'" + ifDiscountNotExists + "'",
                "'" + EnumStringConverter.PrintEnum(PurchaseWay) + "'"
            };
        }
    }
}