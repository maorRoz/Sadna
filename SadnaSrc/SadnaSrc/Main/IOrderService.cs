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

        MarketAnswer CreateOrder();
        MarketAnswer RemoveOrder(int orderId);

        MarketAnswer AddItemToOrder(int orderID, OrderItem item);
        MarketAnswer RemoveItemFromOrder(int orderID, string store, string name);


    }

    public enum OrderStatus
    {
        Success,
        NoOrderWithID,

    }

    public enum OrderItemStatus
    {
        Success,
        NoOrderItemInOrder,
        ItemAlreadyInOrder
    }
}