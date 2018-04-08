using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;

namespace SadnaSrc.MarketHarmony
{
    //TODO: improve this class lior/igor!!
    //integration between OrderPool to StoreCenter 
    public interface IStoresSyncher
    {
        void CloseLottery(); // improve this
        void RemoveProducts(OrderItem[] purchased);

        bool IsValid(OrderItem toBuy);
    }
}
