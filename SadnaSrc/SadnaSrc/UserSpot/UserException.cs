using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Constraints;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    class UserException : MarketException
    {
        private static int _systemID = -1;
        public UserException(EnterSystemStatus status, string message) : base((int) status, message)
        {
        }

        public UserException(SignUpStatus status, string message) : base((int)status, message)
        {
        }

        public UserException(SignInStatus status, string message) : base((int)status, message)
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
