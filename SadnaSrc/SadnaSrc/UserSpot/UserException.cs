using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.UserSpot
{
    class UserException : MarketException
    {

        public UserException(MarketError error,string message) : base(error, message) { }
        public UserException(EnterSystemStatus status, string message) : base((int) status, message)
        {
        }

        public UserException(ViewUsersStatus status, string message) : base((int)status, message)
        {
        }

	    public UserException(ViewStoresStatus status, string message) : base((int) status, message)
	    {

	    }

        public UserException(GetControlledStoresStatus status, string message) : base((int)status, message)
        {
        }

        public UserException(GetStoreManagerPoliciesStatus status, string message) : base((int) status, message)
        {

        }

        public UserException(SignUpStatus status, string message) : base((int)status, message)
        {
        }

        public UserException(SignInStatus status, string message) : base((int)status, message)
        {
        }
        public UserException(ViewCartStatus status, string message) : base((int)status, message)
        {

        }
        public UserException(EditCartItemStatus status, string message) : base((int)status, message)
        {
        }

        public UserException(RemoveFromCartStatus status, string message) : base((int)status, message)
        {
        }

        public UserException(PromoteStoreStatus status, string message) : base((int)status, message)
        {
        }

        public UserException(BrowseMarketStatus status, string message) : base((int)status, message)
        {
        }

        public UserException(ManageMarketSystem status, string message) : base((int)status, message)
        {
        }

	    public UserException(GetEntranceDetailsEnum status, string message) : base((int)status, message)
	    {
	    }

		protected override string GetModuleName()
        {
            return "UserSpot";
        }
    }
}
