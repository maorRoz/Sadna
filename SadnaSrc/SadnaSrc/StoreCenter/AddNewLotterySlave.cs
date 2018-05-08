using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class AddNewLotterySlave : AbstractStoreCenterSlave
    {
        public MarketAnswer answer;
        private readonly Store store;

        public AddNewLotterySlave(string storeName, IUserSeller storeManager,IStoreDL storeDL) : base(storeName, storeManager, storeDL)
        {
            store = DataLayerInstance.GetStorebyName(storeName);
        }

        public StockListItem AddNewLottery(string name, double price, string description, DateTime startDate, DateTime endDate)
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
                CheckIfProductNameAvailable(name);
                MarketLog.Log("StoreCenter", "check that dates are OK");
                CheckIfDatesAreOK(startDate, endDate);
                MarketLog.Log("StoreCenter", "Dates are fine");
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
                answer = new StoreAnswer(StoreEnum.NoPermission, "you have no premmision to do that");
            }
            return null;
        }

        private static void CheckIfDatesAreOK(DateTime startDate, DateTime endDate)
        {
            if (startDate >= MarketYard.MarketDate && startDate < endDate) return;
            MarketLog.Log("StoreCenter", "something wrong with the dates");
            throw new StoreException(StoreEnum.DatesAreWrong, "dates are not leagal");
        }

        private void CheckIfProductNameAvailable(string name)
        {
            Product product = DataLayerInstance.GetProductByNameFromStore(_storeName, name);
            if (product != null)
                throw new StoreException(StoreEnum.ProductNameNotAvlaiableInShop, "product name must be uniqe per shop");
        }
    }
}