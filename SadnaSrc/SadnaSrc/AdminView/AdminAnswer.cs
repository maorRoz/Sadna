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

        public AdminAnswer(ViewSystemLogStatus status, string answer,string[] report) : base((int)status, answer,report)
        {

        }

        public AdminAnswer(ViewSystemErrorStatus status, string answer, string[] report) : base((int)status, answer, report)
        {

        }

        public AdminAnswer(ViewSystemLogStatus status, string answer) : base((int)status, answer)
        {

        }

        public AdminAnswer(ViewSystemErrorStatus status, string answer) : base((int)status, answer)
        {

        }

        public AdminAnswer(ViewPurchaseHistoryStatus status, string answer, string[] historyReport) : base((int)status, answer,historyReport)
        {
        }

        public AdminAnswer(EditCategoryStatus status, string answer) : base((int) status, answer)
        {
        }

        public AdminAnswer(EditPolicyStatus status, string answer) : base((int)status, answer)
        {
        }

        public AdminAnswer(ViewPolicyStatus status, string answer,string[] policyIds) : base((int)status, answer, policyIds)
        {
        }

        public AdminAnswer(ViewPolicyStatus status, string answer) : base((int)status, answer)
        {
        }

	    public AdminAnswer(GetEntranceDetailsEnum status, string answer, string[] policyIds) : base((int)status, answer, policyIds)
	    {
	    }

	    public AdminAnswer(GetEntranceDetailsEnum status, string answer) : base((int)status, answer)
	    {
	    }

		public AdminAnswer(AdminException e) : base(e.Status, e.Message)
        {
        }

    }
}
