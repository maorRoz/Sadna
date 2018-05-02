using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class AddNewProductSlave : AbstractStoreCenterSlave
    {
        internal MarketAnswer answer;
        private Store _store;
        public AddNewProductSlave(IUserSeller storeManager, string _StoreName) : base(_StoreName, storeManager)
        {
            _store = DataLayerInstance.getStorebyName(_storeName);
        }

        internal StockListItem AddNewProduct(string _name, double _price, string _description, int quantity)
        {

            MarketLog.Log("StoreCenter", "trying to add product to store");
            MarketLog.Log("StoreCenter", "check if store exists");
            try
            {
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to add products");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if product name avlaiable in the store" + _storeName);
                checkIfProductNameAvailable(_name);
                
                MarketLog.Log("StoreCenter", " name is avlaiable");
                MarketLog.Log("StoreCenter", " checking that quanitity is positive");
                _checkQuantityIsOK(quantity);
                MarketLog.Log("StoreCenter", " quanitity is positive");
                Product product = new Product(_name, _price, _description);
                StockListItem stockListItem = new StockListItem(quantity, product, null, PurchaseEnum.Immediate, _store.SystemId);
                DataLayerInstance.AddStockListItemToDataBase(stockListItem);
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

        private void checkIfProductNameAvailable(string name)
        {
            Product P = DataLayerInstance.getProductByNameFromStore(_storeName, name);
            if (P != null)
                throw new StoreException(StoreEnum.ProductNameNotAvlaiableInShop, "product name must be uniqe per shop");
        }

        private void _checkQuantityIsOK(int quantity)
        {
            if (quantity <= 0)
            { throw new StoreException(StoreEnum.quantityIsNegatie, "negative quantity"); }
        }

    }
}