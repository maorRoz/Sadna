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

        MarketAnswer BuyItemWithCoupon(string itemName, string store, int quantity, double unitPrice, string coupon);

        MarketAnswer BuyLotteryTicket(string itemName, string store, int quantity,double unitPrice);

        MarketAnswer BuyAllItemsFromStore(string store);
        
        MarketAnswer BuyEverythingFromCart();

        MarketAnswer GiveDetails(string userName, string address, string creditCard);

        void CleanSession();
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
        InvalidCoupon

    }

    public enum OrderItemStatus
    {
        Success,
        NoOrderItemInOrder,
        ItemAlreadyInOrder,
        InvalidDetails
    }

    public enum LotteryOrderStatus
    {
        Success,
        InvalidLotteryID,
        InvalidLotteryTicket
    }
}
