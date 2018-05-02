using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class ChangeProductPurchaseWayToLotterySlave : AbstractStoreCenterSlave
    {
        internal MarketAnswer answer;

        public ChangeProductPurchaseWayToLotterySlave(string storeName, IUserSeller storeManager) : base (storeName,storeManager)
        {
        }

        internal void ChangeProductPurchaseWayToLottery(string productName, DateTime startDate, DateTime endDate)
        {
            try
            {
                MarketLog.Log("StoreCenter", "check if store exists");
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " check if has premmision to edit products");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", "check if product exists");
                checkifProductExists(DataLayerInstance.GetProductByNameFromStore(_storeName, productName));
                MarketLog.Log("StoreCenter", "product exists");
                StockListItem stockListItem = DataLayerInstance.GetProductFromStore(_storeName, productName);
                CheckIfAlreadyLottery(stockListItem);
                MarketLog.Log("StoreCenter", "check if dates are OK");
                CheckIfDatesAreOK(startDate, endDate);
                stockListItem.PurchaseWay = PurchaseEnum.Lottery;
                DataLayerInstance.EditStockInDatabase(stockListItem);
                LotterySaleManagmentTicket lotterySaleManagmentTicket = new LotterySaleManagmentTicket(
                    _storeName, stockListItem.Product, startDate, endDate);
                DataLayerInstance.AddLottery(lotterySaleManagmentTicket);

                answer = new StoreAnswer(ChangeToLotteryEnum.Success, "type changed");
            }
            catch (StoreException exe)
            {
                answer = new StoreAnswer(exe);
            }
            catch (MarketException)
            {
                answer = new StoreAnswer(StoreEnum.NoPremmision, "you have no premmision to do that");
            }

        }

        private void CheckIfDatesAreOK(DateTime startDate, DateTime endDate)
        {
            if (startDate.Date > endDate.Date)
                throw new StoreException(ChangeToLotteryEnum.DatesAreWrong, "start date is lated then end date");
        }

        private void CheckIfAlreadyLottery(StockListItem stockListItem)
        {
            if (stockListItem.PurchaseWay == PurchaseEnum.Lottery)
            {
                MarketLog.Log("StoreCenter", " product has a lottery");
                throw new StoreException(ChangeToLotteryEnum.LotteryExists, "product has a lottery");
            }
        }
    }
}