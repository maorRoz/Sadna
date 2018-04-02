using SadnaSrc.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    class StoreAnswer : MarketAnswer
    {
        public StoreAnswer(UpdateStockStatus status, string answer) : base((int)status, answer) { }

    }
}