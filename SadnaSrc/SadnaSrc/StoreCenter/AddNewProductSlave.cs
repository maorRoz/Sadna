using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class AddNewProductSlave
    {
        internal MarketAnswer answer;
        StoreDL global;
        private IUserSeller _storeManager;
        string _storeName;
        Store _store;
        private int globalProductID;
        public AddNewProductSlave(IUserSeller storeManager, Store store)
        {
            _storeManager = storeManager;
            _store = store;
            _storeName = store.Name;
        }

        internal StockListItem AddNewProduct(string _name, double _price, string _description, int quantity)
        {

            MarketLog.Log("StoreCenter", "trying to add product to store");
            MarketLog.Log("StoreCenter", "check if store exists");
            checkIfStoreExistsAndActive();
            try
            {
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to add products");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if product name avlaiable in the store" + _storeName);
                
                MarketLog.Log("StoreCenter", " name is avlaiable");
                MarketLog.Log("StoreCenter", " checking that quanitity is positive");
                _checkQuantityIsOK(quantity);
                MarketLog.Log("StoreCenter", " quanitity is positive");
                Product product = new Product(GetProductID(), _name, _price, _description);
                StockListItem stockListItem = new StockListItem(quantity, product, null, PurchaseEnum.Immediate, store.SystemId);
                global.AddStockListItemToDataBase(stockListItem);
                MarketLog.Log("StoreCenter", "product added");
                answer = new StoreAnswer(StoreEnum.Success, "product added");
                return stockListItem;
            }
            catch (StoreException exe)
            {
                answer = new StoreAnswer(exe);
                return null;
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                answer = new StoreAnswer(StoreEnum.NoPremmision, "you have no premmision to do that");
                return null;
            }
        }

        private string GetProductID()
        {
            int currentMaxProductId = globalProductID;
            globalProductID++;
            return "P" + currentMaxProductId;
        }

        private void _checkQuantityIsOK(int quantity)
        {
            if (quantity <= 0)
            { throw new StoreException(StoreEnum.quantityIsNegatie, "negative quantity"); }
        }

        private void checkIfStoreExistsAndActive()
        {
            if (!global.IsStoreExistAndActive(_storeName))
            { throw new StoreException(StoreEnum.StoreNotExists, "store not exists or active"); }
        }

        private void IsProductNameAvailableInStore(string name)
        {

            Product P = global.getProductByNameFromStore(_storeName, name);
            if (P == null)
            { throw new StoreException(StoreEnum.ProductNameNotAvlaiableInShop, "Product Name is already Exists In Shop"); }
        }
    }
}