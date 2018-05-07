using System.Collections.Generic;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class RemoveProductFromCategorySlave : AbstractStoreCenterSlave
    {
        public MarketAnswer Answer { get; set; }

        public RemoveProductFromCategorySlave(string storeName, IUserSeller storeManager, IStoreDL storedl) : base(
            storeName, storeManager, storedl)
        {
        }



        public void RemoveProductFromCategory(string categoryName, string productName)
        {

            try
            {
                MarketLog.Log("StoreCenter", "trying to remove product from category in the store");
                MarketLog.Log("StoreCenter", "check if store exists");
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to handle categorys");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if category name exists in the store " + _storeName);
                string storeid = GetStoreIDbyName();
                CheckIfCategoryExistsInStore(categoryName, storeid);
                MarketLog.Log("StoreCenter", "Check if Product exists in store");
                var product = DataLayerInstance.GetProductByNameFromStore(_storeName, productName);
                checkifProductExists(product);
                MarketLog.Log("StoreCenter", "Product exists");
                MarketLog.Log("StoreCenter", "Check if product in this category");
                Category category = DataLayerInstance.GetCategoryByName(categoryName);
                CheckifProductInCategory(product, category.SystemId);
                MarketLog.Log("StoreCenter", "Product is in category");
                DataLayerInstance.RemoveProductFromCategory(category.SystemId, product.SystemId);
                Answer = new StoreAnswer(StoreEnum.Success,
                    "product" + productName + " removed successfully from category" + categoryName);
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

        private void CheckifProductInCategory(Product product, string categoryName)
        {
            LinkedList<Product> products = DataLayerInstance.GetAllCategoryProducts(categoryName);
            bool check = false;
            foreach (Product aProduct in products)
            {
                if (product.Equals(aProduct))
                {
                    check = true;
                }
            }

            if (!check)
            {
                throw new StoreException(StoreEnum.ProductNotInCategory, "Product not exists in category, ");
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
            Category category = DataLayerInstance.GetCategoryByName(categoryName);
            if (category == null)
            {
                throw new StoreException(StoreEnum.CategoryNotExistsInStore, "category not exists in the store");
            }
        }
    }
}
