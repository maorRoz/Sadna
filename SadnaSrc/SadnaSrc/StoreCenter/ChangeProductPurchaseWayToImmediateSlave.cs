using System;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class ChangeProductPurchaseWayToImmediateSlave : AbstractStoreCenterSlave
    {
        public MarketAnswer answer;
        private IOrderSyncher syncher;

        public ChangeProductPurchaseWayToImmediateSlave(string storeName, IUserSeller storeManager,
            IOrderSyncher _syncher, IStoreDL storeDL) : base(storeName, storeManager, storeDL)
        {
            syncher = _syncher;
        }

        public void ChangeProductPurchaseWayToImmediate(string productName)
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
        }

        private void ValidateLottery(StockListItem stockList)
        {
            if (stockList.PurchaseWay != PurchaseEnum.Lottery) return;
            LotterySaleManagmentTicket lottery = DataLayerInstance.GetLotteryByProductID(stockList.Product.SystemId);
            lottery.InformCancel(syncher);
            DataLayerInstance.EditLotteryInDatabase(lottery);
        }
    }
}