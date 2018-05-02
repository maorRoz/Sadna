using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class AddNewLotterySlave : AbstractStoreCenterSlave
    {
        internal MarketAnswer answer;
        Store store;

        public AddNewLotterySlave(string storeName, IUserSeller storeManager) : base(storeName, storeManager)
        {
            store = DataLayerInstance.getStorebyName(storeName);
        }

        internal StockListItem AddNewLottery(string name, double price, string description, DateTime startDate, DateTime endDate)
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
                checkIfProductNameAvailable(name);
                Product product = new Product(name, price, description);
                StockListItem stockListItem = new StockListItem(1, product, null, PurchaseEnum.Lottery, store.SystemId);
                DataLayerInstance.AddStockListItemToDataBase(stockListItem);
                LotterySaleManagmentTicket lotterySaleManagmentTicket = new LotterySaleManagmentTicket(
                    _storeName, stockListItem.Product, startDate, endDate);
                DataLayerInstance.AddLottery(lotterySaleManagmentTicket);

                MarketLog.Log("StoreCenter", "product added");
                answer = new StoreAnswer(StoreEnum.Success, "product added");
                return stockListItem;
            }
            catch (StoreException exe)
            {
                answer = new StoreAnswer(exe);
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                answer = new StoreAnswer(StoreEnum.NoPremmision, "you have no premmision to do that");
            }
            return null;
        }

        private void checkIfProductNameAvailable(string name)
        {
            Product P = DataLayerInstance.getProductByNameFromStore(_storeName, name);
            if (P != null)
                throw new StoreException(StoreEnum.ProductNameNotAvlaiableInShop, "product name must be uniqe per shop");
        }
    }
}