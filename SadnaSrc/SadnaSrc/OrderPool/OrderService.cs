using System.Collections.Generic;
using SadnaSrc.Main;
using SadnaSrc.MarketFeed;
using SadnaSrc.MarketHarmony;


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

        private readonly LotteryTicketSlave ltSlave;

        public OrderService(IUserBuyer buyer, IStoresSyncher storesSync)
        {
            Orders = new List<Order>();
            _buyer = buyer;
            UserName = buyer.GetName();
            UserAddress = buyer.GetAddress();
            CreditCard = buyer.GetCreditCard();
            _storesSync = storesSync;
            _orderDL = OrderDL.Instance;

            ltSlave = new LotteryTicketSlave(_buyer, _storesSync, _orderDL, MarketYard.Instance.GetPublisher(), MarketYard.Instance.GetPolicyChecker());
        }

        //only for Unit Tests of developer!!(not for integration or blackbox or real usage)
        public void LoginBuyer(string userName, string password)
        {
            ((UserBuyerHarmony)_buyer).LogInBuyer(userName, password);
            UserName = _buyer.GetName();
            UserAddress = _buyer.GetAddress();
            CreditCard = _buyer.GetCreditCard();
        }

        public void Cheat(int cheatResult)
        {
            ltSlave.Cheat(cheatResult);
        }

        /*
         * Interface functions
         */

        public MarketAnswer BuyItemFromImmediate(string itemName, string store, int quantity, double unitPrice, string coupon)
        {
            PurchaseItemSlave piSlave = new PurchaseItemSlave(_buyer, _storesSync, _orderDL, MarketYard.Instance.GetPublisher(), MarketYard.Instance.GetPolicyChecker());
            Order newOrder = piSlave.BuyItemFromImmediate(itemName, store, quantity, unitPrice, coupon, UserName, UserAddress, CreditCard);
            if (newOrder != null)
                Orders.Add(newOrder);
            return piSlave.Answer;
        }


        public MarketAnswer BuyLotteryTicket(string itemName, string store, int quantity, double unitPrice)
        { 
            Order newOrder = ltSlave.BuyLotteryTicket(itemName, store, quantity, unitPrice, UserName, UserAddress, CreditCard);
            if (newOrder != null)
                Orders.Add(newOrder);
            return ltSlave.Answer;
        }


        public MarketAnswer BuyEverythingFromCart(string[] coupons) 
        {
            PurchaseEverythingSlave peSlave = new PurchaseEverythingSlave(_buyer, _storesSync, _orderDL, MarketYard.Instance.GetPublisher(), MarketYard.Instance.GetPolicyChecker());
            Order newOrder = peSlave.BuyEverythingFromCart(coupons, UserName, UserAddress, CreditCard);
            if (newOrder != null)
                Orders.Add(newOrder);
            return peSlave.Answer;
        }
        

        public MarketAnswer GiveDetails(string userName, string address, string creditCard)
        {
            ValidateDetailsSlave vdSlave = new ValidateDetailsSlave();
            vdSlave.GiveDetails(userName, address, creditCard);
            if (vdSlave.Answer.Status == (int) GiveDetailsStatus.Success)
            {
                UserName = userName;
                UserAddress = address;
                CreditCard = creditCard;
            }
            return vdSlave.Answer;
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
