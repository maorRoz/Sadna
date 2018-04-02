using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using SadnaSrc.UserSpot;

namespace SadnaSrc.SupplyPoint
{
    public class SupplyService : ISupplyService
    {
        private OrderService _orderService;

        public SupplyService(UserService userService,OrderService orderService)
        {
            _orderService = orderService;
        }
        public MarketAnswer CreateDelivery(int orderId, string address)
        {
            Order order = _orderService.getOrder(orderId);
            order.setAddress(address);
            // TODO: add external supply system functions when they are ready.
            
            return new SupplyAnswer(SupplyStatus.Success, "Success, A");
        }
    }
}
