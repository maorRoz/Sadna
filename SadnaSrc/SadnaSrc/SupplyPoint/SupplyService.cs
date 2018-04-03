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
        private SupplySystem sock;
        public SupplyService(UserService userService,OrderService orderService)
        {
            _orderService = orderService;
            sock = new SupplySystem();
        }
        public MarketAnswer CreateDelivery(int orderId, string address)
        {
            Order order = _orderService.getOrder(orderId);
            order.setAddress(address);
            // TODO: Find out how the Order Data must reach the delivery system.
            if (sock.ProcessDelivery(orderId, order.GetUserName(), address))
            {
                return new SupplyAnswer(SupplyStatus.Success, "Success, A new delivery was assigned for the requested order");
            }
            throw new SupplyException(SupplyStatus.SupplySystemError, "Failed, an error in the supply system occured.");
        }
    }
}
