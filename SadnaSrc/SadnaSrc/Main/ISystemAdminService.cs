﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SadnaSrc.Main
{
    public interface ISystemAdminService
    {
        MarketAnswer RemoveUser(int userSystemID);
        MarketAnswer ViewPurchaseHistoryByUser(string userName);
        MarketAnswer ViewPurchaseHistoryByStore(string storeName);
    }

    public enum RemoveUserStatus
    {
        Success,
        SelfTermination,
        NotSystemAdmin,
        NoUserFound
    }

    public enum ViewPurchaseHistoryStatus
    {
        Success,
        NotSystemAdmin,
        NoUserFound,
        NoStoreFound
    }
}
