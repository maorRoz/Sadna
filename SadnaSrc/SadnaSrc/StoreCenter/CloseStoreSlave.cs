using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class CloseStoreSlave
    {
        internal MarketAnswer answer;
        private IUserSeller _storeManager;
        public Store store;
        public string _storeName;
        StoreDL global;
        public CloseStoreSlave(IUserSeller storeManager, ref Store _store)
        {
            _storeManager = storeManager;
            store = _store;
            _storeName = store.Name;
            global = StoreDL.Instance;
        }

        internal void closeStore()
        {
            try
            {
                checkIfStoreExists();
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

        private void checkIfStoreExists()
        {
            if (!global.IsStoreExist(_storeName))
            { throw new StoreException(StoreEnum.StoreNotExists, "store not exists"); }
        }
    }
}