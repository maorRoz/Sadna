using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class CloseStoreSlave : AbstractStoreCenterSlave
    {
        public MarketAnswer Answer;
        private Store store;
        public CloseStoreSlave(IUserSeller storeManager, string _storeName, IStoreDL storeDL) : base(_storeName, storeManager, storeDL)
        {
            store = DataLayerInstance.GetStorebyName(_storeName);
        }
        
        public void CloseStore()
        {
            try
            {
                checkIfStoreExistsAndActive();
                _storeManager.CanPromoteStoreOwner(); 
                Answer = store.CloseStore();
            }
            catch (StoreException exe)
            {
                MarketLog.Log("StoreCenter", "closing store failed");
                Answer = new StoreAnswer(exe);
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "closing store failed");
                Answer = new StoreAnswer(StoreEnum.CloseStoreFail, "you have no premmision to do that");
            }
        }
    }
}