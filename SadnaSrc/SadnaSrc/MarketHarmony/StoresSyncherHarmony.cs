using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.OrderPool;

namespace SadnaSrc.MarketHarmony
{
    //TODO: improve this class igor/lior/zohar!!!
    class StoresSyncherHarmony : IStoresSyncher
    {
        public StoresSyncherHarmony()
        {

        }
        public void CloseLottery(string lottery)
        {
            throw new NotImplementedException();
        }

        public void RemoveProducts(OrderItem[] purchased)
        {
            throw new NotImplementedException();
        }

        public bool IsValid(OrderItem toBuy)
        {
            throw new NotImplementedException();
        }
    }
}
