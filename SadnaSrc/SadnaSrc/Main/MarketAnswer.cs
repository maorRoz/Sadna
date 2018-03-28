using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.UserSpot;

namespace SadnaSrc.Main
{
    public class MarketAnswer
    {
        public int Status { get; private set; }
        public string Answer { get; private set; }
        public MarketAnswer(int status, string answer)
        {
            Status = status;
            Answer = answer;
        }

    }
}
