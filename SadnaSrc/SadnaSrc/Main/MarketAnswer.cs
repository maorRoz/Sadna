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
        public int Status { get; }
        public string Answer { get; }

        public string[] ReportList { get; }
        public MarketAnswer(int status, string answer)
        {
            Status = status;
            Answer = answer;
            ReportList = null;
        }

        public MarketAnswer(int status, string answer, string[] report)
        {
            Status = status;
            Answer = answer;
            ReportList = report;
        }

    }

}
