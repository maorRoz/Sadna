using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.SupplyPoint;
using SadnaSrc.Walleter;

namespace SadnaSrc.OrderPool
{
    public abstract class MakePurchaseSlave
    {
        protected readonly IOrderDL _orderDL;

        protected readonly IUserBuyer _buyer;
        protected readonly IStoresSyncher _storesSync;

        protected readonly SupplyService _supplyService;
        protected readonly PaymentService _paymentService;

        public MakePurchaseSlave(IUserBuyer buyer, IStoresSyncher storesSync, IOrderDL orderDL)
        {
            _buyer = buyer;
            _storesSync = storesSync;
            _supplyService = SupplyService.Instance;
            _paymentService = PaymentService.Instance;
            _orderDL = orderDL;

        }

        protected void CheckOrderItem(OrderItem item)
        {
            if (item.Name == null || item.Store == null || item.Quantity == 0)
            {
                MarketLog.Log("OrderPool", "User entered item details which are invalid by the system standards!");
                throw new OrderException(OrderItemStatus.InvalidDetails, "User entered invalid item details");
            }
        }

        protected abstract void ProcessOrder(Order order, string CreditCard);
    }
}
