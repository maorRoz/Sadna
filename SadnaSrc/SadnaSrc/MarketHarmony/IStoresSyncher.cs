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
        /// <summary>
        /// Close a lottery sale in store in case of lottery sold out
        /// </summary>
        void CloseLottery(); // improve this
        /// <summary>
        /// Rmove <paramref name="purchased"/> from store stock
        /// </summary>
        void RemoveProducts(OrderItem[] purchased);
        /// <summary>
        /// Validate that <paramref name="toBuy"/> is valid product to buy by the user
        /// </summary>
        bool IsValid(OrderItem toBuy);
    }
}
