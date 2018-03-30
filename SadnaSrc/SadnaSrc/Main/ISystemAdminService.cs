using System;
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
        MarketAnswer ViewPurchaseHistory(int userSystemID);
        MarketAnswer ViewPurchaseHistory(string storeName);
    }

    public enum RemoveUserStatus
    {
        Success,
        NotSystemAdmin,
        NoUserFound
    }

    public enum ViewPurchaseHistoryStatus
    {
        Success,
        NotSystemAdmin
    }
}
