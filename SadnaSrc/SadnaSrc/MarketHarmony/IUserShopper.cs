using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketHarmony
{
    interface IUserShopper
    {
        void AddToCart(/* product */);

        bool AddOwnership(string store);

    }
}
