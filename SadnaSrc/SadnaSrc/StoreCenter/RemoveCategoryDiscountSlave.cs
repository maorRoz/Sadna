using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class RemoveCategoryDiscountSlave : AbstractStoreCenterSlave
    {
        public MarketAnswer Answer { get; set; }
        public RemoveCategoryDiscountSlave(string storeName, IUserSeller storeManager, IStoreDL storeDl) : base(storeName, storeManager, storeDl)
        {
        }

        public void RemoveCategoryDiscount(string categoryName)
        {
            try
            {
                
            MarketLog.Log("StoreCenter", "trying to remove discount from category in store");
            MarketLog.Log("StoreCenter", "check if store exists");
            checkIfStoreExistsAndActive();
            MarketLog.Log("StoreCenter", " check if has premmision to edit products");
            _storeManager.CanDeclareDiscountPolicy();
            MarketLog.Log("StoreCenter", " has premmission");
            MarketLog.Log("StoreCenter", " check that cateory exists");
            CheckIfCategoryExists(categoryName);
            MarketLog.Log("StoreCenter", "category exists");
            MarketLog.Log("StoreCenter", " check that category has discount in this store");
            CheckHasExistsDiscount(categoryName);
            CategoryDiscount categoryDiscount = DataLayerInstance.GetCategoryDiscount(categoryName, _storeName);
            DataLayerInstance.RemoveCategoryDiscount(categoryDiscount);
            MarketLog.Log("StoreCenter", "categoryDiscountd added successfully");
            Answer = new StoreAnswer(StoreEnum.Success, "categoryDiscountd removed successfully");
        }
        catch (StoreException exe)
        {
            Answer = new StoreAnswer((StoreEnum) exe.Status, exe.GetErrorMessage());
        }
        catch (MarketException)
        {
            Answer = new StoreAnswer(StoreEnum.NoPermission, "you have no premmision to do that");
        }
        catch (DataException e)
        {
            Answer = new StoreAnswer((StoreEnum) e.Status, e.GetErrorMessage());
        }
        }
        private void CheckIfCategoryExists(string categoryName)
        {
            Category category = DataLayerInstance.GetCategoryByName(categoryName);
            if (category == null)
            {
                throw new StoreException(StoreEnum.CategoryNotExistsInSystem, "category not exists in the store");
            }
        }
        private void CheckHasExistsDiscount(string categoryName)
        {
            CategoryDiscount categoryDiscount = DataLayerInstance.GetCategoryDiscount(categoryName, _storeName);
            if (categoryDiscount == null)
            {
                throw new StoreException(StoreEnum.CategoryDiscountNotExistsInStore, "categorydiscount not exists in the store");
            }
        }
    }
}