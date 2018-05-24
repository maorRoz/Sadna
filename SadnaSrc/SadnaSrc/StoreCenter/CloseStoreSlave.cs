using System;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class CloseStoreSlave : AbstractStoreCenterSlave
    {
        public MarketAnswer answer;
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
                answer = store.CloseStore();
            }
            catch (StoreException exe)
            {
                MarketLog.Log("StoreCenter", "closing store failed");
                answer = new StoreAnswer(exe);
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "closing store failed");
                answer = new StoreAnswer(StoreEnum.CloseStoreFail, "you have no premmision to do that");
            }
            catch (DataException e)
            {
                answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
            }
        }
    }
}