using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public interface IStoreService
    {
        MarketAnswer OpenStore();

        MarketAnswer PromoteToOwner();
    }
    public enum UpdateStockStatus
    {
        Success,
        Fail
    }
}
