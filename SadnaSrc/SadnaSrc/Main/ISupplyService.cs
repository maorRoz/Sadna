using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public interface ISupplyService
    {
        MarketAnswer CreateDelivery(int OrderId, string address);
    }

    public enum SupplyStatus
    {
        Success,
        SupplySystemError,

    }
}
