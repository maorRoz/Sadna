using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketFeed;
using SadnaSrc.MarketHarmony;
using SadnaSrc.PolicyComponent;
using SadnaSrc.SupplyPoint;
using SadnaSrc.Walleter;

namespace SadnaSrc.OrderPool
{
    public abstract class MakePurchaseSlave
    {
        protected readonly IOrderDL _orderDL;

        protected readonly IUserBuyer _buyer;
        protected readonly IStoresSyncher _storesSync;
        protected readonly IPublisher _publisher;

        protected readonly SupplyService _supplyService;
        protected readonly PaymentService _paymentService;
        protected readonly IPolicyChecker _checker;

        public MakePurchaseSlave(IUserBuyer buyer, IStoresSyncher storesSync, IOrderDL orderDL,IPublisher publisher, IPolicyChecker checker)
        {
            _buyer = buyer;
            _storesSync = storesSync;
            _supplyService = SupplyService.Instance;
            _paymentService = PaymentService.Instance;
            _orderDL = orderDL;
            _publisher = publisher;
            _checker = checker;
        }

        protected void CheckOrderItem(OrderItem item)
        {
            if (item.Name == null || item.Store == null || item.Quantity == 0)
            {
                MarketLog.Log("OrderPool", "User entered item details which are invalid by the system standards!");
                throw new OrderException(OrderItemStatus.InvalidDetails, "User entered invalid item details");
            }
        }

        public void CheckPurchasePolicy(Order order)
        {
            string username = order.GetUserName();
            string address = order.GetShippingAddress();
            List<OrderItem> items = order.GetItems();
            foreach (OrderItem item in items)
            {
                if (!_checker.CheckRelevantPolicies(item.Name, item.Store, null, username, address, item.Quantity,
                    item.Price))
                    throw new OrderException(OrderItemStatus.NotComplyWithPolicy,
                        "Item " + item.Name + "from Store" + item.Store + "Doesn't comply with Policy conditions.");
            }
        }

        protected abstract void ProcessOrder(Order order, string CreditCard);
    }
}
