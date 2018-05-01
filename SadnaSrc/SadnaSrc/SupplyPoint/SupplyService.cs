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
        private readonly SupplySystem sock = SupplySystem.Instance;


        private static SupplyService _instance;

        public static SupplyService Instance => _instance ?? (_instance = new SupplyService());


        public void CreateDelivery(Order order)
        {
            if (sock == null)
            {
                throw new SupplyException(SupplyStatus.NoSupplySystem, "Failed, No supply system found.");
            }
            MarketLog.Log("SupplyPoint", "Attempting to create delivery for order ID: "+ order.GetOrderID());
            CheckOrderDetails(order);
            if (sock.ProcessDelivery(order.GetOrderID(), order.GetUserName(), order.GetShippingAddress()))
            {
                MarketLog.Log("SupplyPoint", "Delivery for order ID: "+ order.GetOrderID() + " was successufully assigned.");
                return;
            }
            throw new SupplyException(SupplyStatus.SupplySystemError, "Failed, an error in the supply system occured.");
        }

        public void CheckOrderDetails(Order order)
        {
            if (order.GetOrderID() < 100000 || order.GetOrderID() > 999999
                                            || order.GetUserName() == null ||
                                            order.GetShippingAddress() == null ||
                                            order.GetItems().Count == 0)
            {
                throw new SupplyException(SupplyStatus.InvalidOrder,"Failed, Invalid order details");
            }
        }

        public void BreakExternal()
        {
            sock.fuckUp();
        }

        public void FixExternal()
        {
            sock.fuckDown();
        }
    }
}
