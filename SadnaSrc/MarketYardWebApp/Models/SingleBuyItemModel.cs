using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketYardWebApp.Models;

namespace MarketYardWebApp.Models
{
    public class SingleBuyItemModel : UserModel
    {
        public string Store { get; set; }
        public string Product { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double FinalPrice { get; set; }
        public string UserName { get; set; }
        public string UserAddress { get; set; }
        public string UserCreditCard { get; set; }
        public SingleBuyItemModel(int systemId, string state, string message, string store,
            string product, double unitPrice, int quantity, double finalPrice,
            string userName,string userAddress,string userCreditCard) : base(systemId, state, message)
        {
            Store = store;
            Product = product;
            UnitPrice = unitPrice;
            Quantity = quantity;
            FinalPrice = finalPrice;
            UserName = userName;
            UserAddress = userAddress;
            UserCreditCard = userCreditCard;
        }
    }
}
