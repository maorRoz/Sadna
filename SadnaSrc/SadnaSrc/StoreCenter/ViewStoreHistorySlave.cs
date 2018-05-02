﻿using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class ViewStoreHistorySlave : AbstractStoreCenterSlave
    {
        internal MarketAnswer answer;
        private Store store;
        
        public ViewStoreHistorySlave(string _store, IUserSeller storeManager) :base (_store, storeManager)
        {
            store = DataLayerInstance.getStorebyName(_store);
        }

        internal void ViewStoreHistory()
        {
            MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " attempting to view the store purchase history...");
            try
            {
                DataLayerInstance.IsStoreExistAndActive(_storeName);
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
            catch (MarketException e)
            {
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " has no permission to view purchase history in Store"
                                             + _storeName + " and therefore has been denied. Error message has been created!");
                answer = new StoreAnswer(ManageStoreStatus.InvalidManager, e.GetErrorMessage());
            }
        }
    }
}