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
        public UserException(string message) : base(message)
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

        protected override string GetErrorMessage(string message)
        {
            return "User " + _systemID + " Error: " + message;
        }
    }
}
