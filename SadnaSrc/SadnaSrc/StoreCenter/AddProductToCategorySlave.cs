using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class AddProductToCategorySlave : AbstractStoreCenterSlave
    {
        public MarketAnswer Answer { get; set; }
        public AddProductToCategorySlave(string storeName, IUserSeller storeManager, IStoreDL storeDL) : base(storeName, storeManager, storeDL)
        {
        }
        public void AddProductToCategory(string categoryName, string productName)
        {
             try
             {
                MarketLog.Log("StoreCenter", "trying to add product to category to in the store");
                MarketLog.Log("StoreCenter", "check if store exists");
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to handle categorys");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if category name exists in the store " + _storeName);
                CheckIfCategoryExists(categoryName);
                MarketLog.Log("StoreCenter", "Check if Product exists in store");
                Product P = DataLayerInstance.GetProductByNameFromStore(_storeName, productName);
                checkifProductExists(P);
                MarketLog.Log("StoreCenter", "Product exists");
                MarketLog.Log("StoreCenter", "Check if product not in this category already");
                Category category = DataLayerInstance.GetCategoryByName(categoryName);
                CheckifProductNotInCategory(P, category.SystemId);
                MarketLog.Log("StoreCenter", "Product not alrady exists in category");
                DataLayerInstance.AddProductToCategory(category.SystemId, P.SystemId);
                Answer = new StoreAnswer(StoreEnum.Success, "product " + productName + " add successfully to category " + categoryName);
             }
            catch (StoreException e)
             {
                Answer = new StoreAnswer((StoreEnum)e.Status,e.GetErrorMessage());
             }
             catch (DataException e)
             {
                 Answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
             }
            catch (MarketException)
             {
                Answer = new StoreAnswer(StoreEnum.NoPermission, "you have no premmision to do that");
             }
        }

        private void CheckifProductNotInCategory(Product P, string categoryID)
        {
            LinkedList<Product> products = DataLayerInstance.GetAllCategoryProducts(categoryID);
            foreach (Product product in products)
            {
                if (P.Equals(product))
                {
                    throw new StoreException(StoreEnum.ProductAlreadyInCategory, "Product alrady exists in category, ");
                }
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
            Category category = DataLayerInstance.GetCategoryByName(categoryName);
            if (category == null)
            {
                throw new StoreException(StoreEnum.CategoryNotExistsInStore, "category not exists in the store");
            }
        }
    }
}