using SadnaSrc.SupplyPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Walleter;
using SadnaSrc.Main;
using SadnaSrc.MarketFeed;

namespace SadnaSrc.OrderPool
{
    class StoreOrderTools
    {
        private readonly OrderDL _orderDL;
        private SupplyService _supplyService;
        private PaymentService _paymentService;

        private List<Order> Orders;


        public StoreOrderTools()
        {
            _orderDL = OrderDL.Instance;
            _supplyService = SupplyService.Instance;
            _paymentService = PaymentService.Instance;

            Orders = new List<Order>();
        }

        public void RefundAllExpiredLotteries()
        {
            MarketLog.Log("OrderPool", "Attempting to refund expired lotteries...");
            string[] expiredLotteries = _orderDL.GetAllExpiredLotteries();
            foreach (string lottery in expiredLotteries)
            {
               RefundLottery(lottery);
            }
        }

        public void RefundLottery(string lottery)
        {
            MarketLog.Log("OrderPool", "Attempting to refund lottery " + lottery + "...");
            try
            {
                string[] ticketsToRefund = _orderDL.GetAllTickets(lottery);
                List<int> refundedIds = new List<int>();
                foreach (string ticket in ticketsToRefund)
                {
                    var refundedId = Refund(ticket);
                    refundedIds.Add(refundedId);
                }

                var publisher = MarketYard.Instance.GetPublisher();
                publisher.NotifyLotteryCanceled(refundedIds.ToArray());
                _orderDL.CancelLottery(lottery);

            }
            catch (OrderException)
            {
                MarketLog.Log("OrderPool", "Refund " + lottery + " has failed to execute. Error message has been created!");
            }
            catch (WalleterException)
            {
                MarketLog.Log("OrderPool", "Refund " + lottery + " has failed  to execute." +
                                            " communication with payment system is inturrepted." + " Error message has been created!");
            }
            catch (SupplyException)
            {
                MarketLog.Log("OrderPool", "Refund " + lottery + " has failed  to execute." +
                                            " communication with supply system is inturrepted." + " Error message has been created!");
            }
            catch (MarketException)
            {
                MarketLog.Log("OrderPool", "Refund " + lottery + " has failed to execute. Something is wrong with Store or User." +
                                            " Error message has been created!");
            }
        }



        private int Refund(string ticket)
        {
            int participantID = _orderDL.GetTicketParticipantID(ticket);
            if(participantID < 0)
                throw new OrderException(LotteryOrderStatus.InvalidLotteryTicket, "Cannot find ticket or user");
            string creditCardToRefund = _orderDL.GetCreditCardToRefund(participantID);
            string nameToRefund = _orderDL.GetNameToRefund(participantID);
            double sumToRefund = _orderDL.GetSumToRefund(ticket);
            Order order = RefundOrder(sumToRefund, nameToRefund, ticket); // should be ticket later
            _paymentService.Refund(sumToRefund, creditCardToRefund, nameToRefund);
            Orders.Add(order);
            _orderDL.AddOrder(order,"Lottery");
            _orderDL.RemoveTicket(ticket);
            MarketLog.Log("OrderPool", "User " + nameToRefund + " successfully refunded the sum: " + sumToRefund);
            return participantID;
        }

        public void SendPackage(string itemName, string store,int userId)
        {
            MarketLog.Log("OrderPool", "Attempting to send package...");
            int orderId = 0;
            try
            {
                OrderItem toBuy = new OrderItem(store, "DELIVERY : " + itemName, 1, 1);
                Order order = InitOrder(_orderDL.GetNameToRefund(userId), _orderDL.GetAddressToSendPackage(userId));
                orderId = order.GetOrderID();
                order.AddOrderItem(toBuy);
                _supplyService.CreateDelivery(order);
                Orders.Add(order);
                _orderDL.AddOrder(order,"Lottery");
                MarketLog.Log("OrderPool", "Successfully made delivery for item: " + itemName);
            }
            catch (OrderException)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Error message has been created!");
            }
            catch (WalleterException)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with payment system." +
                                           " Error message has been created!");
            }
            catch (SupplyException)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
            }
            catch (MarketException)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Something is wrong with Store or User." +
                                           " Error message has been created!");
            }
        }

        private Order InitOrder(string userName,string userAddress)
        {
            if(userName == null || userAddress == null)
                throw new OrderException(OrderItemStatus.InvalidDetails, "Cannot find name or address");
            Order order = new Order(_orderDL.RandomOrderID(), userName, userAddress);
            MarketLog.Log("OrderPool", " successfully initialized new order " + order.GetOrderID() + "for user "+userName+".");
            return order;
        }

        private Order RefundOrder(double sum,string userName,string ticket)
        {
            if(sum <= 0 || userName == null || ticket == null)
                throw new OrderException(OrderItemStatus.InvalidDetails, "Cannot find cost or user name.");
            Order refund = new Order(_orderDL.RandomOrderID(), userName);
            refund.AddOrderItem(new OrderItem("---", "REFUND: Lottery Ticket", -1 * sum, 1)); 
            MarketLog.Log("OrderPool", " successfully initialized new order " + refund.GetOrderID() + "for user " + userName + ".");

            return refund;
        }
    }
}
