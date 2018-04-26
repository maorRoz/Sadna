using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class ViewStoreInfoSlave
    {
        internal MarketAnswer answer;
        private IUserShopper _shopper;
        StoreDL storeLogic;

        public ViewStoreInfoSlave(IUserShopper shopper)
        {
            _shopper = shopper;
            storeLogic = StoreDL.Instance;
        }

        internal void ViewStoreInfo(string store)
        {
            try
            {
                MarketLog.Log("StoreCenter", "check that have premission to view store info");
                _shopper.ValidateCanBrowseMarket();
                MarketLog.Log("StoreCenter", "premission gained");
                string[] storeInfo = storeLogic.GetStoreInfo(store);
                MarketLog.Log("StoreCenter", "info gained");
                answer = new StoreAnswer(ViewStoreStatus.Success, "Store info has been successfully granted!", storeInfo);
            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "");
                answer = new StoreAnswer((ViewStoreStatus)e.Status, "Something is wrong with viewing " + store +
                                                                  " info by customers . Error message has been created!");
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                answer = new StoreAnswer(ViewStoreStatus.InvalidUser,
                    "User validation as valid customer has been failed . only valid users can browse market. Error message has been created!");
            }
        }
    }
}