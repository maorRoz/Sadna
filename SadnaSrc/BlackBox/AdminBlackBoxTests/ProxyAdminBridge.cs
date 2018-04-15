using System;
using SadnaSrc.Main;

namespace BlackBox
{
    class ProxyAdminBridge : IAdminBridge
    {
        public IAdminBridge real;

        public void GetAdminService(IUserService userService)
        {
            if (real != null)
            {
                real.GetAdminService(userService);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public MarketAnswer RemoveUser(string userName)
        {
            if (real != null)
            {
                return real.RemoveUser(userName);
            }
            throw new NotImplementedException();
        }


        public MarketAnswer ViewPurchaseHistoryByUser(string userName)
        {
            if (real != null)
            {
                return real.ViewPurchaseHistoryByUser(userName);
            }
            throw new NotImplementedException();
        }

        public MarketAnswer ViewPurchaseHistoryByStore(string storeName)
        {
            if (real != null)
            {
                return real.ViewPurchaseHistoryByStore(storeName);
            }
            throw new NotImplementedException();
        }
    }
}