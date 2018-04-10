using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketHarmony
{
    interface IOrderSyncher
    {
        //integration from StoreShoppingService(StoreCenter module) to OrderPool module
        void BuyLotteryTicket(string buyerName, string creditCard, double priceToPay);
    }
}
