using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using SadnaSrc.UserSpot;


using ProxyExternalSystems;


namespace SadnaSrc.SupplyPoint
{
    public class SupplyService : ISupplyService
    {
        private OrderService _orderService;
        private SupplySystem sock = null;

        public SupplyService(UserService userService,OrderService orderService)
        {
            _orderService = orderService;
        }

        //TODO: change this once info about external systems is available.
        public MarketAnswer AttachExternalSystem()
        {
            sock = new SupplySystem();
            return new SupplyAnswer(SupplyStatus.Success, "Success, External payment system attached.");

        }

        public MarketAnswer CreateDelivery(int orderId, string address)
        {
            if (sock == null)
            {
                throw new SupplyException(SupplyStatus.NoSupplySystem, "Failed, an error in the supply system occured.");
            }
            Order order = _orderService.getOrder(orderId);
            order.setAddress(address);
            // TODO: Find out how the Order Data must reach the delivery system.
            if (sock.ProcessDelivery(orderId, order.GetUserName(), address))
            {
                MarketLog.Log("SupplyPoint", "Delivery for order ID: "+ orderId+" was successufully assigned.");
                return new SupplyAnswer(SupplyStatus.Success, "Success, A new delivery was assigned for the requested order");
            }
            throw new SupplyException(SupplyStatus.SupplySystemError, "Failed, an error in the supply system occured.");
        }

        // Method for testing only WILL BE REMOVED
        public void FuckUpExternal()
        {
            sock.fuckUp();
        }
    }
}
