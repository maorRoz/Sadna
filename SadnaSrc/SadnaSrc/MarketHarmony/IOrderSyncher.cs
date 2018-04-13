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
        /// TODO: might need to add more arguments
        /// </summary>
        /// 
        void CloseLottery(string lottery); // improve this


        /// <summary>
        /// Close a lottery sale in store in case of lottery expires and not sold out
        /// </summary>
        void CancelLottery(string lottery);

    }
}
