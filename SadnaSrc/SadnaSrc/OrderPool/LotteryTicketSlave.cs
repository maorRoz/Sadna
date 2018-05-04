using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketFeed;
using SadnaSrc.MarketHarmony;
using SadnaSrc.SupplyPoint;
using SadnaSrc.Walleter;

namespace SadnaSrc.OrderPool
{
    public class LotteryTicketSlave : MakePurchaseSlave
    {
        private int cheatCode = -1;

        public OrderAnswer Answer { get; private set; }

        public LotteryTicketSlave(IUserBuyer buyer, IStoresSyncher storesSync, IOrderDL orderDL,IPublisher publisher) :
            base(buyer, storesSync, orderDL,publisher){}

        public Order BuyLotteryTicket(string itemName, string store, int quantity, double unitPrice, string UserName,
            string UserAddress, string CreditCard)
        {
            if (UserName == null)
                UserName = _buyer.GetName();
            if (UserAddress == null)
                UserAddress = _buyer.GetAddress();
            if (CreditCard == null)
                CreditCard = _buyer.GetCreditCard();
            MarketLog.Log("OrderPool", "Attempting to buy " + quantity + " tickets for lottery sale of " + itemName +
                                       " from store " + store + "...");
            int orderId = 0;
            try
            {
                _buyer.ValidateRegisteredUser();
                _storesSync.ValidateTicket(itemName, store, unitPrice);
                OrderItem ticketToBuy = new OrderItem(store, itemName, unitPrice, quantity);
                Order order = CreateOrderOneItem(ticketToBuy, UserName, UserAddress);
                orderId = order.GetOrderID();
                ProcessOrder(order, CreditCard);
                _storesSync.UpdateLottery(itemName, store, unitPrice, UserName, cheatCode);
                MarketLog.Log("OrderPool", "User " + UserName + " successfully bought lottery ticket.");
                Answer = new OrderAnswer(OrderStatus.Success, "Successfully bought Lottery ticket ");
                _publisher.NotifyClientBuy(store,itemName);
                return order;
            }
            catch (OrderException e)
            {
                MarketLog.Log("OrderPool",
                    "Order " + orderId + " has failed to execute. Error message has been created!");
                Answer = new OrderAnswer((OrderStatus) e.Status, e.GetErrorMessage());
                return null;
            }
            catch (WalleterException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId +
                                           " has failed to execute while communicating with payment system." +
                                           " Error message has been created!");
                Answer = new OrderAnswer((WalleterStatus) e.Status, e.GetErrorMessage());
                return null;
            }
            catch (SupplyException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId +
                                           " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
                Answer = new OrderAnswer((SupplyStatus) e.Status, e.GetErrorMessage());
                return null;
            }
            catch (MarketException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId +
                                           " has failed to execute. Something is wrong with Store or User." +
                                           " Error message has been created!");
                Answer = new OrderAnswer(OrderStatus.InvalidUser, e.GetErrorMessage());
                _publisher.NotifyClientBuy(store, itemName);
                return null;
            }
        }

        public Order InitOrder(string UserName, string UserAddress)
        {
            Order order = new Order(_orderDL.RandomOrderID(), UserName, UserAddress);
            MarketLog.Log("OrderPool",
                "User " + UserName + " successfully initialized new order " + order.GetOrderID() + ".");
            return order;
        }

        public void Cheat(int cheatResult)
        {
            cheatCode = cheatResult;
        }

        private Order CreateOrderOneItem(OrderItem toBuy, string UserName,string UserAddress)
        {
            CheckOrderItem(toBuy);
            Order order = InitOrder(UserName, UserAddress);
            order.AddOrderItem(toBuy);
            return order;
        }

        protected override void ProcessOrder(Order order, string CreditCard)
        {
            _paymentService.ProccesPayment(order, CreditCard);
            _orderDL.AddOrder(order, "Lottery");
        }
    }
}
