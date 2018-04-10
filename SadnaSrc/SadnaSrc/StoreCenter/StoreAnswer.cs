using SadnaSrc.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class StoreAnswer : MarketAnswer
    {
        public StoreAnswer(StoreEnum status, string answer) : base((int)status, answer) { }
        public StoreAnswer(PromoteStoreStatus status, string answer) : base((int)status, answer) { }

    }
}