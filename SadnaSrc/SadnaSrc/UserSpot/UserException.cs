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
        private static int _systemID = -1;

        public UserException(MarketError error,string message) : base(error, message) { }
        public UserException(EnterSystemStatus status, string message) : base((int) status, message)
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

        public static void SetUser(int systemID)
        {
            _systemID = systemID;
        }

        protected override string GetModuleName()
        {
            return "UserSpot";
        }

        protected override string WrapErrorMessageForDb(string message)
        {
            return "User " + _systemID + " Error: " + message;
        }
    }
}
