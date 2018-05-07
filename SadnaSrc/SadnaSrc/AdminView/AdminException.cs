using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.AdminView
{
    public class AdminException : MarketException
    {

        public AdminException(MarketError error, string message) : base(error, message) { }
        public AdminException(RemoveUserStatus status, string message) : base((int)status, message)
        {
        }

        public AdminException(ViewPurchaseHistoryStatus status, string message) : base((int)status, message)
        {
        }

        public AdminException(EditCategoryStatus status, string message) : base((int) status, message)
        {
        }

        protected override string GetModuleName()
        {
            return "AdminView";
        }

        protected override string WrapErrorMessageForDb(string message)
        {
            return "System Admin Error: " + message;
        }
    }
}
