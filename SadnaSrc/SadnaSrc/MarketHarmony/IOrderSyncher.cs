using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketHarmony
{
    interface IOrderSyncher
    {


        /// <summary>
        /// Close a lottery sale in store in case of lottery sold out
        /// </summary>
        /// 
        void CloseLottery(string productName, string store, int winnerId);


        /// <summary>
        /// Close a lottery sale in store in case of lottery expires and not sold out
        /// </summary>
        void CancelLottery(string lottery);

        void CleanSession();

    }
}