using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class AddNewProductSlave : AbstractSlave
    {
        internal MarketAnswer answer;
        private Store _store;
        private int globalProductID;
        private All_ID_Manager manager;
        public AddNewProductSlave(IUserSeller storeManager, Store store) : base(store.Name, storeManager)
        {
            _store = store;
            manager = All_ID_Manager.GetInstance();
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
                Product product = new Product(manager.GetProductID(), _name, _price, _description);
                StockListItem stockListItem = new StockListItem(quantity, product, null, PurchaseEnum.Immediate, _store.SystemId);
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
        
        private void _checkQuantityIsOK(int quantity)
        {
            if (quantity <= 0)
            { throw new StoreException(StoreEnum.quantityIsNegatie, "negative quantity"); }
        }

    }
}