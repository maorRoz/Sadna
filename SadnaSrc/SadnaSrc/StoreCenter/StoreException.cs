using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.StoreCenter
{
    class StoreException : MarketException
    {
        public StoreException(int status, string message) : base(status, message)
        {
        }

        public StoreException(ViewPurchaseHistoryStatus status, string message) : base((int)status, message)
        {
        }

        protected override string GetModuleName()
        {
            return "StoreCenter";
        }

        protected override string WrapErrorMessageForDb(string message)
        {
            // TODO: implement a better message wrapper like in UserException
            return " " + message;
        }
    }
}
