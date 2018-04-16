﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.OrderPool;

namespace SadnaSrc.MarketHarmony
{
    public class OrderSyncherHarmony : IOrderSyncher
    {
        StoreOrderTools tools;
        public OrderSyncherHarmony()
        {
            tools = new StoreOrderTools();
        }

        public void CloseLottery(string productName, string store, int winnerId)
        {
            tools.SendPackage(productName, store, winnerId);
        }

        public void CancelLottery(string lottery)
        {
            tools.RefundLottery(lottery);
        }

        public void CleanSession()
        {
            tools.CleanSession();
        }
    }
}