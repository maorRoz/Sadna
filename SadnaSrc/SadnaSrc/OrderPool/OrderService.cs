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


        public readonly List<Order> Orders;
        private readonly OrderDL _orderDL;
        
        private readonly IUserBuyer _buyer;
        private readonly IStoresSyncher _storesSync;

        private readonly MakePurchaseSlave slave;



        public OrderService(IUserBuyer buyer, IStoresSyncher storesSync)
        {
            Orders = new List<Order>();
            _buyer = buyer;
            _storesSync = storesSync;
            _orderDL = OrderDL.Instance;

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
            UserName = _buyer.GetName();
            UserAddress = _buyer.GetAddress();
            CreditCard = _buyer.GetCreditCard();
        }

        public Order InitOrder(OrderItem[] items)
        {
            Order order = new Order(_orderDL.RandomOrderID(), UserName, UserAddress);
            foreach (OrderItem item in items)
            {
                order.AddOrderItem(item);
            }

            Orders.Add(order);
            MarketLog.Log("OrderPool", "User " + UserName + " successfully initialized new order " + order.GetOrderID() + ".");
            return order;
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
        
        public Order GetOrder(int orderID)
        {
            foreach (Order order in Orders)
            {
                if (order.GetOrderID() == orderID) return order;
            }
            return null;
        }

        /*
         * Interface functions
         */

        public MarketAnswer BuyItemFromImmediate(string itemName, string store, int quantity, double unitPrice)
        {
            Order newOrder = slave.BuyItemFromImmediate(itemName, store, quantity, unitPrice);
            if(newOrder!= null)
                Orders.Add(newOrder);
            return slave.Answer;
        }

        public MarketAnswer BuyItemWithCoupon(string itemName, string store, int quantity, double unitPrice, string coupon)
        {
            Order newOrder = slave.BuyItemWithCoupon(itemName, store, quantity, unitPrice, coupon);
            if (newOrder != null)
                Orders.Add(newOrder);
            return slave.Answer;
        }


        public MarketAnswer BuyLotteryTicket(string itemName, string store, int quantity, double unitPrice)
        {
            Order newOrder = slave.BuyLotteryTicket(itemName, store, quantity, unitPrice);
            if (newOrder != null)
                Orders.Add(newOrder);
            return slave.Answer;
        }


        public MarketAnswer BuyAllItemsFromStore(string store)
        {
            Order newOrder = slave.BuyAllItemsFromStore(store);
            if (newOrder != null)
                Orders.Add(newOrder);
            return slave.Answer;
        }


        public MarketAnswer BuyEverythingFromCart()
        {
            Order newOrder = slave.BuyEverythingFromCart();
            if (newOrder != null)
                Orders.Add(newOrder);
            return slave.Answer;
        }
        

        public MarketAnswer GiveDetails(string userName, string address, string creditCard)
        {
            slave.GiveDetails(userName, address, creditCard);
            if (slave.Answer.Status == (int)GiveDetailsStatus.Success)
                slave.UpdateUserDetails(userName, address, creditCard);
            return slave.Answer;
        }
    }
}
