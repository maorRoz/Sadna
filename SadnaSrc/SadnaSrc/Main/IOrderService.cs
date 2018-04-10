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
        MarketAnswer BuyItemFromImmediate(string itemName, string store, int quantity, double unitPrice);

        MarketAnswer BuyLotteryTicket(string itemName, string store, int quantity,double unitPrice);

        MarketAnswer BuyAllItemsFromStore(string store);

        MarketAnswer BuyEverythingFromCart();


        //TODO: i think that the problem start from here.
        //TODO: maybe its because you thought in the first place that this is something the client should ask from you(Everything in comments).
       /* MarketAnswer CreateOrder(out int orderId);  
        MarketAnswer RemoveOrder(int orderId);

        MarketAnswer AddItemToOrder(int orderID, string store, string name, double price, int quantity);
        MarketAnswer RemoveItemFromOrder(int orderID, string store, string name);*/


    }

    public enum GiveDetailsStatus
    {
        Success,
        InvalidNameOrAddress
    }
    public enum OrderStatus
    {
        Success,
        InvalidUser,
        InvalidNameOrAddress,
        NoSupplyConnection,
        NoPaymentConnection,
        NoOrderWithID,

    }

    public enum OrderItemStatus
    {
        Success,
        NoOrderItemInOrder,
        ItemAlreadyInOrder
    }
}
