using SadnaSrc.SupplyPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Walleter;
using SadnaSrc.Main;

namespace SadnaSrc.OrderPool
{
    class StoreOrderTools
    {
        private readonly OrderPoolDL _orderDL;
        private SupplyService _supplyService;
        private PaymentService _paymentService;

        private List<Order> Orders;


        public StoreOrderTools()
        {
            _orderDL = new OrderPoolDL();
            _supplyService = SupplyService.Instance;
            _paymentService = PaymentService.Instance;

            _supplyService.AttachExternalSystem();
            _paymentService.AttachExternalSystem();

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
                foreach (string ticket in ticketsToRefund)
                {
                    Refund(ticket);
                }

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



        private void Refund(string ticket)
        {
            int orderId = 0;
            int participantID = _orderDL.GetTicketParticipantID(ticket);
            string creditCardToRefund = _orderDL.GetCreditCardToRefund(participantID);
            string nameToRefund = _orderDL.GetNameToRefund(participantID);
            double sumToRefund = _orderDL.GetSumToRefund(ticket);
            Order order = RefundOrder(sumToRefund, nameToRefund, ticket);
            _paymentService.Refund(sumToRefund, creditCardToRefund, nameToRefund);
            Orders.Add(order);
            _orderDL.AddOrder(order,"Lottery");
            _orderDL.RemoveTicket(ticket);
            MarketLog.Log("OrderPool", "User " + nameToRefund + " successfully refunded the sum: " + sumToRefund);
        }

        public void SendPackage(string itemName, string store,int userId)
        {
            MarketLog.Log("OrderPool", "Attempting to send package...");
            int orderId = 0;
            try
            {
                OrderItem toBuy = new OrderItem(store, "DELIVERY : " + itemName, 1, 1);
                OrderService.CheckOrderItem(toBuy);
                Order order = InitOrder(_orderDL.GetNameToRefund(userId), _orderDL.GetAddressToSendPackage(userId));
                orderId = order.GetOrderID();
                order.AddOrderItem(toBuy);
                _supplyService.CreateDelivery(order);
                Orders.Add(order);
                _orderDL.AddOrder(order,"Lottery");
                MarketLog.Log("OrderPool", "Successfully made delivery for item: " + itemName);
            }
            catch (OrderException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Error message has been created!");
            }
            catch (WalleterException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with payment system." +
                                           " Error message has been created!");
            }
            catch (SupplyException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
            }
            catch (MarketException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Something is wrong with Store or User." +
                                           " Error message has been created!");
            }
        }

        public Order InitOrder(string userName,string userAddress)
        {
            Order order = new Order(_orderDL.RandomOrderID(), userName, userAddress);
            MarketLog.Log("OrderPool", " successfully initialized new order " + order.GetOrderID() + "for user "+userName+".");
            return order;
        }

        private Order RefundOrder(double sum,string userName,string ticket)
        {
            Order refund = new Order(_orderDL.RandomOrderID(), userName);
            refund.AddOrderItem(new OrderItem("---", "REFUND: "+ticket, -1 * sum, 1)); 
            MarketLog.Log("OrderPool", " successfully initialized new order " + refund.GetOrderID() + "for user " + userName + ".");

            return refund;
        }

        public void CleanSession()
        {
            foreach (Order order in Orders)
            {
                _orderDL.RemoveOrder(order.GetOrderID());
            }
        }


    }
}
