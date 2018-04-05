using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.OrderPool;

namespace SadnaSrc.MarketHarmony
{
    // integration between UserSpot to OrderPool only
    interface IUserBuyer
    {
        OrderItem[] Checkout();

        string GetAddress();

        string GetName();
    }
}
