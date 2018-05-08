using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.AdminView
{
    public class AdminAnswer : MarketAnswer
    {
        public AdminAnswer(RemoveUserStatus status, string answer) : base((int)status, answer)
        {

        }
        public AdminAnswer(ViewPurchaseHistoryStatus status, string answer, string[] historyReport) : base((int)status, answer,historyReport)
        {
        }

        public AdminAnswer(EditCategoryStatus status, string answer) : base((int) status, answer)
        {
        }
        public AdminAnswer(AdminException e) : base(e.Status, e.Message)
        {
        }

    }
}
