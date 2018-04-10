using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public interface ISupplyService
    {
        //TODO: you shouldn't let the client get any interaction with this interface, no MarketAnswer is needed here
        MarketAnswer AttachExternalSystem();
        MarketAnswer CreateDelivery(int OrderId, string address);
    }

    public enum SupplyStatus
    {
        Success,
        SupplySystemError,
        NoSupplySystem
    }
}
