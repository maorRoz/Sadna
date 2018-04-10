using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.SupplyPoint
{
    class SupplyException : MarketException
    {
        public SupplyException(SupplyStatus status, string message) : base((int)status, message)
        {
        }


        protected override string GetModuleName()
        {
            return "SupplyPoint";
        }

        protected override string WrapErrorMessageForDb(string message)
        {
            return " " + message;
        }
    }
}
