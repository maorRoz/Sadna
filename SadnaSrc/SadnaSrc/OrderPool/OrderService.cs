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

        private readonly OrderPoolSlave slave;

        public OrderService(IUserBuyer buyer, IStoresSyncher storesSync)
        {
            Orders = new List<Order>();
            _buyer = buyer;
            UserName = buyer.GetName();
            UserAddress = buyer.GetAddress();
            CreditCard = buyer.GetCreditCard();
            _storesSync = storesSync;
            _orderDL = OrderDL.Instance;

            slave = new OrderPoolSlave(buyer, storesSync, OrderDL.Instance);
        }

        //only for Unit Tests of developer!!(not for integration or blackbox or real usage)
        public void LoginBuyer(string userName, string password)
        {
            ((UserBuyerHarmony)_buyer).LogInBuyer(userName, password);
            UserName = _buyer.GetName();
            UserAddress = _buyer.GetAddress();
            CreditCard = _buyer.GetCreditCard();
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
            slave.Cheat(cheatResult);
        }

        /*
         * Interface functions
         */

        public MarketAnswer BuyItemFromImmediate(string itemName, string store, int quantity, double unitPrice, string coupon)
        {
            Order newOrder = slave.BuyItemFromImmediate(itemName, store, quantity, unitPrice, coupon, UserName, UserAddress, CreditCard);
            if (newOrder != null)
                Orders.Add(newOrder);
            return slave.Answer;
        }


        public MarketAnswer BuyLotteryTicket(string itemName, string store, int quantity, double unitPrice)
        {
            Order newOrder = slave.BuyLotteryTicket(itemName, store, quantity, unitPrice, UserName, UserAddress, CreditCard);
            if (newOrder != null)
                Orders.Add(newOrder);
            return slave.Answer;
        }


        public MarketAnswer BuyEverythingFromCart(string[] coupons) 
        {
            Order newOrder = slave.BuyEverythingFromCart(coupons, UserName, UserAddress, CreditCard);
            if (newOrder != null)
                Orders.Add(newOrder);
            return slave.Answer;
        }
        

        public MarketAnswer GiveDetails(string userName, string address, string creditCard)
        {
            slave.GiveDetails(userName, address, creditCard);
            if (slave.Answer.Status == (int) GiveDetailsStatus.Success)
            {
                UserName = userName;
                UserAddress = address;
                CreditCard = creditCard;
            }
            return slave.Answer;
        }

        //Getter function for specific order from the list
        public Order GetOrder(int orderID)
        {
            foreach (Order order in Orders)
            {
                if (order.GetOrderID() == orderID) return order;
            }
            return null;
        }
    }
}
