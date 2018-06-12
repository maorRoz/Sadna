using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.StoreCenter
{
    public class StoreException : MarketException
    {
        public StoreException(CalculateEnum status, string message) : base((int)status, message)
        {
        }
        public StoreException(MarketError status, string message) : base((int)status, message)
        {
        }
        public StoreException(StoreSyncStatus status, string message) : base((int)status, message)
        {
        }
        public StoreException(OpenStoreStatus status, string message) : base((int)status, message)
        {
        }

        public StoreException(ViewStoreStatus status, string message) : base((int)status, message)
        {
        }
        public StoreException(AddProductStatus status, string message) : base((int)status, message)
        {
        }
        public StoreException(SearchProductStatus status, string message) : base((int) status, message)
        {
        }
        public StoreException(DiscountStatus status, string message) : base((int)status, message)
        {
        }
        public StoreException(StoreEnum status, string message) : base((int)status, message)
        {
        }

        public StoreException(ChangeToLotteryEnum status, string message) : base((int)status, message)
        {
        }

        public StoreException(EditStorePolicyStatus status, string message) : base((int)status, message)
        {
        }
        public StoreException(ViewStorePolicyStatus status, string message) : base((int)status, message)
        {
        }
        protected override string GetModuleName()
        {
            return "StoreCenter";
        }
    }
}