using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.AdminView
{
    class AdminAnswer : MarketAnswer
    {
        public AdminAnswer(RemoveUserStatus status, string answer) : base((int)status, answer)
        {

        }
        public AdminAnswer(ViewPurchaseHistoryStatus status, string answer) : base((int)status, answer)
        {

        }

    }
}
