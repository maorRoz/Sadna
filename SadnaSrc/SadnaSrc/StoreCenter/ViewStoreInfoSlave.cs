using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class ViewStoreInfoSlave
    {
        internal MarketAnswer answer;
        private IUserShopper _shopper;
        ModuleGlobalHandler storeLogic;

        public ViewStoreInfoSlave(IUserShopper shopper)
        {
            _shopper = shopper;
            storeLogic = ModuleGlobalHandler.GetInstance();
        }

        internal void ViewStoreInfo(string store)
        {
            MarketLog.Log("StoreCenter", "check that have premission to view store info");
            _shopper.ValidateCanBrowseMarket();
            MarketLog.Log("StoreCenter", "premission gained");
            string[] storeInfo = storeLogic.GetStoreInfo(store);
            MarketLog.Log("StoreCenter", "info gained");
            answer = new StoreAnswer(ViewStoreStatus.Success, "Store info has been successfully granted!", storeInfo);
        }
    }
}