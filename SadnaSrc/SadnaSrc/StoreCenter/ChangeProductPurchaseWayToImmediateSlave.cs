using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class ChangeProductPurchaseWayToImmediateSlave : AbstractSlave
    {
        internal MarketAnswer answer;
        private string _storeName;
        private IUserSeller _storeManager;
        private IOrderSyncher syncher;

        public ChangeProductPurchaseWayToImmediateSlave(string storeName, IUserSeller storeManager,
            IOrderSyncher _syncher) : base(storeName,storeManager)
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
                checkifProductExists(global.getProductByNameFromStore(_storeName, productName));
                MarketLog.Log("StoreCenter", "product exists");
                StockListItem stockList = global.GetProductFromStore(_storeName, productName);
                doIfLottery(stockList);
                stockList.PurchaseWay = PurchaseEnum.Immediate;
                global.EditStockInDatabase(stockList);
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

        private void doIfLottery(StockListItem stockList)
        {
            if (stockList.PurchaseWay == PurchaseEnum.Lottery)
            {
                LotterySaleManagmentTicket lottery = global.GetLotteryByProductID(stockList.Product.SystemId);
                lottery.InformCancel(syncher);
                global.EditLotteryInDatabase(lottery);

            }
        }
    }
}