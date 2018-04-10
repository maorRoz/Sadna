using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.Walleter
{
    class WalleterAnswer : MarketAnswer
    {
        public WalleterAnswer(WalleterStatus status, string answer) : base((int)status, answer)
        {

        }
    }
}
