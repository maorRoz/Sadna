using SadnaSrc.UserSpot;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.SupplyPoint;
using SadnaSrc.Walleter;

namespace SadnaSrc.OrderPool
{
    public class OrderService : IOrderService
    {
        public string UserName { get; set; }
        public string UserAddress { get; set; }
        public string CreditCard { get; set; }


        public List<Order> Orders;
        private readonly OrderDL _orderDL;
        
        private readonly IUserBuyer _buyer;
        private IStoresSyncher _storesSync;

        private SupplyService _supplyService;
        private PaymentService _paymentService;

        private readonly MakePurchaseSlave slave;



        public OrderService(IUserBuyer buyer, IStoresSyncher storesSync)
        {
            Orders = new List<Order>();
            _buyer = buyer;
            _storesSync = storesSync;
            _supplyService = SupplyService.Instance;
            _paymentService = PaymentService.Instance;
            GetUserDetailsFromBuyer();
            _orderDL = OrderDL.Instance;

            _supplyService.AttachExternalSystem();
            _paymentService.AttachExternalSystem();

            slave = new MakePurchaseSlave(buyer, storesSync);
        }

        public static void CheckOrderItem(OrderItem item)
        {
            if (item.Name == null || item.Store == null || item.Quantity == 0)
            {
                MarketLog.Log("OrderPool", "User entered item details which are invalid by the system standards!");
                throw new OrderException(OrderItemStatus.InvalidDetails, "User entered invalid item details");
            }
        }

        //only for Unit Tests of developer!!(not for integration or blackbox or real usage)
        public void LoginBuyer(string userName, string password)
        {
            ((UserBuyerHarmony)_buyer).LogInBuyer(userName, password);
            GetUserDetailsFromBuyer();
        }

        public Order InitOrder(OrderItem[] items)
        {
            GetUserDetailsFromBuyer();
            CheckAllItems(items);
            Order order = new Order(_orderDL.RandomOrderID(), UserName, UserAddress);
            foreach (OrderItem item in items)
            {
                order.AddOrderItem(item);
            }

            Orders.Add(order);
            MarketLog.Log("OrderPool", "User " + UserName + " successfully initialized new order " + order.GetOrderID() + ".");
            return order;
        }

        public Order InitOrder()
        {
            GetUserDetailsFromBuyer();
            Order order = new Order(_orderDL.RandomOrderID(), UserName, UserAddress);
            Orders.Add(order);
            MarketLog.Log("OrderPool", "User " + UserName + " successfully initialized new order " + order.GetOrderID() + ".");
            return order;
        }

        public void SaveToDB()
        {
            foreach (Order order in Orders)
            {
                _orderDL.AddOrder(order);
            }
        }

        public void CleanSession()
        {
            foreach (Order order in Orders)
            {
                _orderDL.RemoveOrder(order.GetOrderID());
            }

            _storesSync.CleanSession();
            _buyer.CleanSession();
        }

        public void Cheat(int cheatResult)
        {
            //TODO: something. Maybe return some sort of market answer?
        }

        public void RemoveOrderFromDB(int orderId)
        {
            _orderDL.RemoveOrder(orderId);
        }      
        
        public Order GetOrder(int orderID)
        {
            foreach (Order order in Orders)
            {
                if (order.GetOrderID() == orderID) return order;
            }
            return null;
        }

        public Order GetOrderFromDB(int orderID)
        {
            return _orderDL.FindOrder(orderID);
        }

        public OrderItem FindOrderItemInOrder(int orderId, string store, string name)
        {
            foreach (Order order in Orders)
            {
                return order.GetOrderItem(name, store);
            }

            return null;
        }

        /*
         * Interface functions
         */

        public MarketAnswer BuyItemFromImmediate(string itemName, string store, int quantity, double unitPrice)
        {
            slave.UpdateUserDetails(UserName, UserAddress, CreditCard);
            Order newOrder = slave.BuyItemFromImmediate(itemName, store, quantity, unitPrice);
            if(newOrder!= null)
                Orders.Add(newOrder);
            return slave.Answer;
        }

        public MarketAnswer BuyItemWithCoupon(string itemName, string store, int quantity, double unitPrice, string coupon)
        {
            slave.UpdateUserDetails(UserName, UserAddress, CreditCard);
            Order newOrder = slave.BuyItemWithCoupon(itemName, store, quantity, unitPrice, coupon);
            if (newOrder != null)
                Orders.Add(newOrder);
            return slave.Answer;
        }


        public MarketAnswer BuyLotteryTicket(string itemName, string store, int quantity, double unitPrice)
        {
            slave.UpdateUserDetails(UserName, UserAddress, CreditCard);
            Order newOrder = slave.BuyLotteryTicket(itemName, store, quantity, unitPrice);
            if (newOrder != null)
                Orders.Add(newOrder);
            return slave.Answer;
        }


        public MarketAnswer BuyAllItemsFromStore(string store)
        {
            slave.UpdateUserDetails(UserName, UserAddress, CreditCard);
            Order newOrder = slave.BuyAllItemsFromStore(store);
            if (newOrder != null)
                Orders.Add(newOrder);
            return slave.Answer;
        }


        public MarketAnswer BuyEverythingFromCart()
        {
            slave.UpdateUserDetails(UserName, UserAddress, CreditCard);
            Order newOrder = slave.BuyEverythingFromCart();
            if (newOrder != null)
                Orders.Add(newOrder);
            return slave.Answer;
        }
        

        public MarketAnswer GiveDetails(string userName, string address, string creditCard)
        {
	        OrderDetailsSlave detailSlave = new OrderDetailsSlave();
            detailSlave.GiveDetails(userName, address, creditCard);
            if (detailSlave.Answer.Status == (int)GiveDetailsStatus.Success)
                slave.UpdateUserDetails(userName, address, creditCard);
            return detailSlave.Answer;
        }
        /*
         * Private Functions
         */

        private void GetUserDetailsFromBuyer()
        {
            if (_buyer.GetName() != null)
            {
                UserName = _buyer.GetName();
                UserAddress = _buyer.GetAddress();
                CreditCard = _buyer.GetCreditCard();
            }
        }


        private void CheckAllItems(OrderItem[] items)
        {
            if (items.Length == 0)
            {
                MarketLog.Log("OrderPool", "User entered empty item list !");
                throw new OrderException(OrderItemStatus.InvalidDetails, "User entered empty item list");
            }
            foreach (var item in items)
            {
                CheckOrderItem(item);
            }
        }
    }
}
