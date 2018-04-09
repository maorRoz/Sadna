﻿using System;
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

        MarketAnswer Refund(double sum);

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
