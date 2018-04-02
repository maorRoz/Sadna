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
        public SupplyException(int status, string message) : base(status, message)
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
