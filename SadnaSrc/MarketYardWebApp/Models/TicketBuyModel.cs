using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketYardWebApp.Models;

namespace MarketYardWebApp.Models
{
    public class TicketBuyModel : UserModel
    {
        public string Store { get; set; }
        public string Product { get; set; }

        public double RealPrice { get; set; }
        public string UserName { get; set; }
        public string UserAddress { get; set; }
        public string UserCreditCard { get; set; }
        public TicketBuyModel(int systemId, string state, string message,string store,string product,double realPrice,
             string userName, string userAddress, string userCreditCard) : base(systemId, state, message)
        {
            Store = store;
            Product = product;
            RealPrice = realPrice;
            UserName = userName;
            UserAddress = userAddress;
            UserCreditCard = userCreditCard;
        }
    }
}
