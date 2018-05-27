using System;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;
using SadnaSrc.UserSpot;

namespace SadnaSrc.StoreCenter
{
    public class AddNewLotterySlave : AbstractStoreCenterSlave
    {
        public MarketAnswer answer;

        public AddNewLotterySlave(string storeName, IUserSeller storeManager,IStoreDL storeDL) : base(storeName, storeManager, storeDL)
        {
        }

        public StockListItem AddNewLottery(string name, double price, string description, DateTime startDate, DateTime endDate)
        {
            try
            {
                Store store = DataLayerInstance.GetStorebyName(_storeName);
                MarketLog.Log("StoreCenter", "trying to add product to store");
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " check if has premmision to add products");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", " check if product name avlaiable in the store" + _storeName);
                CheckIfProductNameAvailable(name);
                MarketLog.Log("StoreCenter", "check that dates are OK");
                CheckIfDatesAreOK(startDate, endDate);
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
                answer = new StoreAnswer((StoreEnum)exe.Status,exe.GetErrorMessage());
            }
            catch (MarketException)
            {
                answer = new StoreAnswer(StoreEnum.NoPermission, "you have no premmision to do that");
            }
            catch (DataException e)
            {
                answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
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