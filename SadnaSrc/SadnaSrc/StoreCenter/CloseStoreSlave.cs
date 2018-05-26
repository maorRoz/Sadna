using System;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class CloseStoreSlave : AbstractStoreCenterSlave
    {
        public MarketAnswer answer;
        public CloseStoreSlave(IUserSeller storeManager, string _storeName, IStoreDL storeDL) : base(_storeName, storeManager, storeDL)
        {
        }
        
        public void CloseStore()
        {
            try
            {
                Store store = DataLayerInstance.GetStorebyName(_storeName);
                checkIfStoreExistsAndActive();
                _storeManager.CanPromoteStoreOwner(); 
                answer = store.CloseStore();
            }
            catch (StoreException exe)
            {
                answer = new StoreAnswer(exe);
            }
            catch (MarketException)
            {
                answer = new StoreAnswer(StoreEnum.CloseStoreFail, "you have no premmision to do that");
            }
            catch (DataException e)
            {
                answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
            }
        }
    }
}