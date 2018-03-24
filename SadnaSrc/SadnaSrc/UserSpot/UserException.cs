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
        public UserException(string message) : base(message)
        {
            moduleName = "UserSpot";
        }
        protected override string GetModuleName()
        {
            return "UserSpot";
        }
    }
}
