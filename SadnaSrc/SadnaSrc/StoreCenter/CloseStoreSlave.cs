using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class CloseStoreSlave : AbstractStoreCenterSlave
    {
        internal MarketAnswer answer;
        public Store store;
        public CloseStoreSlave(IUserSeller storeManager, ref string _storeName) : base(_storeName, storeManager)
        {
            store = DataLayerInstance.GetStorebyName(_storeName);
            DataLayerInstance = StoreDL.GetInstance();
        }

        internal void CloseStore()
        {
            try
            {
                checkIfStoreExistsAndActive();
            }
            catch (Exception)
            {
                answer = new StoreAnswer(StoreEnum.StoreNotExists, "the store doesn't exists");
            }
            try
            {
                _storeManager.CanPromoteStoreOwner(); // can do anything
                answer = store.CloseStore();
            }
            catch (StoreException)
            {
                MarketLog.Log("StoreCenter", "closing store failed");
                answer = new StoreAnswer(StoreEnum.CloseStoreFail, "Store is not active");
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "closing store failed");
                answer = new StoreAnswer(StoreEnum.CloseStoreFail, "you have no premmision to do that");
            }
        }
    }
}