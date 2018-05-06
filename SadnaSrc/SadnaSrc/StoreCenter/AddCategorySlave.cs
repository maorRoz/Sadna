﻿
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class AddCategorySlave : AbstractStoreCenterSlave
    {
        public MarketAnswer Answer;
        public AddCategorySlave(string storeName, IUserSeller storeManager, IStoreDL storeDl) : base(storeName, storeManager, storeDl)
        { }

        public Category AddCategory(string categoryName)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to add category to the store");
                MarketLog.Log("StoreCenter", "check if store exists");
                checkIfStoreExistsAndActive();

                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to handle categorys");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if category name exists in the store " + _storeName);
                string storeid = GetStoreIDbyName();
                CheckIfCategoryExists(categoryName);
                MarketLog.Log("StoreCenter", " adding category");
                Category category = new Category(categoryName, storeid);
                DataLayerInstance.AddCategory(category);
                Answer = new StoreAnswer(StoreEnum.Success, "category"+categoryName+" added successfully");
                return category;
            }
            catch (StoreException e)
            {
                Answer = new StoreAnswer(e);
                return null;
            }
            catch (MarketException)
            {
                Answer = new StoreAnswer(StoreEnum.NoPremmision, "you have no premmision to do that");
                return null;
            }
        }

        private string GetStoreIDbyName()
        {
            Store store = DataLayerInstance.GetStorebyName(_storeName);
            if (store == null)
                throw new StoreException(StoreEnum.StoreNotExists, "store not exists");
            return store.SystemId;
        }

        private void CheckIfCategoryExists(string categoryName)
        {
            Category category = DataLayerInstance.getCategoryByName(categoryName);
            if (category != null)
            {
                throw new StoreException(StoreEnum.CategoryExistsInStore, "category exists in the store");
            }
        }
    }
}