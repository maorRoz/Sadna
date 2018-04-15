﻿using SadnaSrc.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class StoreAnswer : MarketAnswer
    {
        public StoreAnswer(StoreEnum status, string answer) : base((int)status, answer) { }
        public StoreAnswer(StoreEnum status, string answer, string[] report) : base((int)status, answer, report) { }
        public StoreAnswer(PromoteStoreStatus status, string answer) : base((int)status, answer) { }
        public StoreAnswer(DiscountStatus status, string answer) : base((int)status, answer) { }
        public StoreAnswer(OpenStoreStatus status, string answer) : base((int)status, answer) { }

        public StoreAnswer(ViewStoreStatus status, string answer, string[] storeReport) : base((int)status, answer,storeReport) { }
        public StoreAnswer(ViewStoreStatus status, string answer) : base((int)status, answer) { }

        public StoreAnswer(ViewStorePurchaseHistoryStatus status, string answer, string[] storeReport) : base((int)status, answer, storeReport) { }
        public StoreAnswer(ViewStorePurchaseHistoryStatus status, string answer) : base((int)status, answer) { }

        public StoreAnswer(AddProductStatus status, string answer) : base((int)status, answer) { }

        public StoreAnswer(AddLotteryTicketStatus status, string answer) : base((int)status, answer) { }

        public StoreAnswer(ManageStoreStatus status, string answer) : base((int)status, answer) { }

        public StoreAnswer(ManageStoreStatus status,string answer, string[] report) : base((int)status,answer, report) { }
        public StoreAnswer(StoreException e) : base((int)e.Status, e.Message) { }
        public StoreAnswer(ChangeToLotteryEnum status, string answer) : base((int)status, answer) { }
    }
}