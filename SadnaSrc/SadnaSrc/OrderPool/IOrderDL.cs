using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.OrderPool
{
    public interface IOrderDL
    {
        int RandomOrderID();
        Order FindOrder(int orderId);
        List<Order> GetAllOrders();
        List<OrderItem> GetAllItems(int orderId);
        OrderItem FindOrderItemInOrder(int orderId, string store, string name);
        List<OrderItem> FindOrderItemsFromStore(string store);
        void AddOrder(Order order);
        void AddOrder(Order order, string saleType);
        void RemoveOrder(int orderId);
        void AddItemToOrder(int orderId, OrderItem item);
        void RemoveItemFromOrder(int orderId, string name, string store);
        void UpdateOrderPrice(int orderId, double price);
        string[] GetAllExpiredLotteries();
        void CancelLottery(string lottery);
        string[] GetAllTickets(string lottery);
        int GetTicketParticipantID(string ticket);
        string GetCreditCardToRefund(int userID);
        string GetNameToRefund(int userID);
        string GetAddressToSendPackage(int userID);
        double GetSumToRefund(string ticket);
        void RemoveTicket(string ticket);
    }
}
