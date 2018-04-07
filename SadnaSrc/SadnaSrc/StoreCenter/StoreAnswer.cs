using SadnaSrc.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    public class StoreAnswer : MarketAnswer
    {
        public StoreAnswer(StoreEnum status, string answer) : base((int)status, answer) { }

    }
}