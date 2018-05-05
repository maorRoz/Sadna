using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class RemoveCategorySlave : AbstractStoreCenterSlave
    {
        public MarketAnswer Answer { get; set; }

        public RemoveCategorySlave(string storeName, IUserSeller storeManager, IStoreDL storedl) : base(storeName,storeManager, storedl)
        {
        }
        public void RemoveCategory(string categoryName)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to remove category from the store");
                MarketLog.Log("StoreCenter", "check if store exists");
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to handle categorys");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if category name exists in the store " + _storeName);
                string storeid = GetStoreIDbyName();
                CheckIfCategoryExistsInStore(categoryName, storeid);
                MarketLog.Log("StoreCenter", " removing category");
                Category category = DataLayerInstance.getCategoryByName(storeid, categoryName);
                DataLayerInstance.RemoveCategory(category);
                Answer = new StoreAnswer(StoreEnum.Success, "category" + categoryName + " removed successfully");
            }
            catch (StoreException e)
            {
                Answer = new StoreAnswer(e);
            }
            catch (MarketException)
            {
                Answer = new StoreAnswer(StoreEnum.NoPremmision, "you have no premmision to do that");
            }
        }
        private string GetStoreIDbyName()
        {
            Store store = DataLayerInstance.GetStorebyName(_storeName);
            if (store == null)
                throw new StoreException(StoreEnum.StoreNotExists, "store not exists");
            return store.SystemId;
        }

        private void CheckIfCategoryExistsInStore(string categoryName, string storeid)
        {
            Category category = DataLayerInstance.getCategoryByName(storeid, categoryName);
            if (category == null)
            {
                throw new StoreException(StoreEnum.CategoryNotExistsInStore, "category not exists in the store");
            }
        }

    }
}