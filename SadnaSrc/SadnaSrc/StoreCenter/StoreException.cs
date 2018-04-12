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
        public StoreException(MarketError status, string message) : base((int)status, message)
        {
        }
        public StoreException(StoreSyncStatus status, string message) : base((int)status, message)
        {
        }
        public StoreException(OpenStoreStatus status, string message) : base((int)status, message)
        {
        }
        public StoreException(ViewPurchaseHistoryStatus status, string message) : base((int)status, message)
        {
        }

        public StoreException(ViewStoreStatus status, string message) : base((int)status, message)
        {
        }

        public StoreException(AddProductStatus status, string message) : base((int)status, message)
        {
        }

        public StoreException(AddLotteryTicketStatus status, string message) : base((int)status, message)
        {
        }
        public StoreException(StoreEnum status, string message) : base((int)status, message)
        {
        }

        public StoreException(ViewStorePurchaseHistoryStatus status, string message) : base((int)status, message)
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
