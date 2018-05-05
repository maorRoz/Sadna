using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketServer.Models;

namespace MarketWeb.Models
{
    public class BuyAllCartModel : CartModel
    {
        public string UserName { get; set; }
        public string UserAddress { get; set; }
        public string UserCreditCard { get; set; }
        public BuyAllCartModel(int systemId, string state, string message, string[] itemData
            ,string userName, string userAddress, string userCreditCard) : base(systemId, state, message, itemData)
        {
            UserName = userName;
            UserAddress = userAddress;
            UserCreditCard = userCreditCard;
        }
    }
}
