using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;


using ProxyExternalSystems;


namespace SadnaSrc.SupplyPoint
{
    public class SupplyService : ISupplyService
    {
        private SupplySystem sock = null;
  

        //TODO: change this once info about external systems is available.
        public void AttachExternalSystem()
        {
            sock = new SupplySystem();
            MarketLog.Log("SupplyPoint", "Connection to external supply system established successfully ! ");

        }

        public void CreateDelivery(Order order)
        {
            if (sock == null)
            {
                throw new SupplyException(SupplyStatus.NoSupplySystem, "Failed, No supply system found.");
            }
            MarketLog.Log("SupplyPoint", "Attempting to create delivery for order ID: "+ order.GetOrderID());
            
            // TODO: Find out how the Order Data must reach the delivery system.
            if (sock.ProcessDelivery(order.GetOrderID(), order.GetUserName(), order.GetShippingAddress()))
            {
                MarketLog.Log("SupplyPoint", "Delivery for order ID: "+ order.GetOrderID() + " was successufully assigned.");
                return;
            }
            throw new SupplyException(SupplyStatus.SupplySystemError, "Failed, an error in the supply system occured.");
        }

        public void BreakExternal()
        {
            sock.fuckUp();
        }
    }
}
