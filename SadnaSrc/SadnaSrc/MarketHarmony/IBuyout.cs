using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketHarmony
{
    interface IBuyout
    {
        // Integration between StoreCenter to UserSpot + OrderPool in case of a buyout with no cart
        void MakeBuyout(/* Product and ????*/);
    }
}
