using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class ChangeProductPurchaseWayToImmediateSlave : AbstractStoreCenterSlave
    {
        internal MarketAnswer answer;
        private IOrderSyncher syncher;

        public ChangeProductPurchaseWayToImmediateSlave(string storeName, IUserSeller storeManager,
            IOrderSyncher _syncher, I_StoreDL storeDL) : base(storeName, storeManager, storeDL)
        {
            syncher = _syncher;
        }

        internal void ChangeProductPurchaseWayToImmediate(string productName)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to edit discount from product in store");
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " check if has premmision to edit product purches type");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", "check if product exists");
                checkifProductExists(DataLayerInstance.GetProductByNameFromStore(_storeName, productName));
                MarketLog.Log("StoreCenter", "product exists");
                StockListItem stockList = DataLayerInstance.GetProductFromStore(_storeName, productName);
                ValidateLottery(stockList);
                stockList.PurchaseWay = PurchaseEnum.Immediate;
                DataLayerInstance.EditStockInDatabase(stockList);
                answer = new StoreAnswer(StoreEnum.Success, "purches way changed");
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

        private void ValidateLottery(StockListItem stockList)
        {
            if (stockList.PurchaseWay == PurchaseEnum.Lottery)
            {
                LotterySaleManagmentTicket lottery = DataLayerInstance.GetLotteryByProductID(stockList.Product.SystemId);
                lottery.InformCancel(syncher);
                DataLayerInstance.EditLotteryInDatabase(lottery);

            }
        }
    }
}