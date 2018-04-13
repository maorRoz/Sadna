using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.OrderPool;

namespace SadnaSrc.MarketHarmony
{
    class OrderSyncherHarmony : IOrderSyncher
    {
        StoreOrderTools tools;
        public OrderSyncherHarmony()
        {
            tools = new StoreOrderTools();
        }

        public void CloseLottery(string productName,string store, int quantity, int winnerId)
        {

            tools.SendPackage(productName,store,quantity,winnerId);
        }

        public void CancelLottery(string lottery)
        {
            tools.RefundLottery(lottery);
        }
    }
}
