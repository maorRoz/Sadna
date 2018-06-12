using System;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class ViewStoreHistorySlave : AbstractStoreCenterSlave
    {
        public MarketAnswer answer;
        
        public ViewStoreHistorySlave(string _store, IUserSeller storeManager, IStoreDL storeDL) : base(_store, storeManager, storeDL)
        {
        }

        public void ViewStoreHistory()
        {
            try
            {
                Store store = DataLayerInstance.GetStorebyName(_storeName);
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " attempting to view the store purchase history...");
                checkIfStoreExistsAndActive();
                _storeManager.CanViewPurchaseHistory();
                var historyReport = DataLayerInstance.GetHistory(store);
                answer =  new StoreAnswer(ViewStorePurchaseHistoryStatus.Success, "View purchase history has been successful!", historyReport);
            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " tried to view purchase history in unavailable Store " + _storeName +
                                             "and has been denied. Error message has been created!");
                answer = new StoreAnswer(ManageStoreStatus.InvalidStore, e.GetErrorMessage());
            }
            catch (DataException e)
            {
                answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " has no permission to view purchase history in Store"
                                             + _storeName + " and therefore has been denied. Error message has been created!");
                answer = new StoreAnswer(ManageStoreStatus.InvalidManager, e.GetErrorMessage());
            }
        }
    }
}