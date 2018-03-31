using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.OrderPool;
using SadnaSrc.UserSpot;

namespace SadnaSrc.Main
{
    public interface IOrderService
    {

        Order CreateOrder(OrderItem[] items);
        Order CreateOrder();
        void RemoveOrder(int orderId);
        void RemoveItemFromOrder(int orderID, string store, string name);
        void AddItemToOrder(int orderID, OrderItem item);
        OrderItem FindOrderItemInOrder(int orderId, string store, string user);
        List<Order> getOrders();
        Order getOrder(int orderID);

    }
}
