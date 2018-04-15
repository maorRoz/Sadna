using SadnaSrc.Main;

namespace BlackBox
{
    interface IAdminBridge
    {
        void GetAdminService(IUserService userService);
        MarketAnswer RemoveUser(string userName);
        MarketAnswer ViewPurchaseHistoryByUser(string userName);
        MarketAnswer ViewPurchaseHistoryByStore(string storeName);
    }
}