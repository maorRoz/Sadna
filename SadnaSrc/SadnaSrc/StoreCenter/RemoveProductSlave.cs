using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class RemoveProductSlave :AbstractStoreCenterSlave
    {
        internal MarketAnswer answer;
        IOrderSyncher syncher;

        public RemoveProductSlave(ref IOrderSyncher _syncher, string name, IUserSeller manager) : base(name, manager)
        {
            syncher = _syncher;
        }

        internal void RemoveProduct(string productName)
        {
            MarketLog.Log("StoreCenter", "trying to remove product from store");
            MarketLog.Log("StoreCenter", "check if store exists");
            try
            {
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to remove products");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if product name exists in the store " + _storeName);
                Product product = DataLayerInstance.getProductByNameFromStore(_storeName, productName);
                checkifProductExists(product);
                MarketLog.Log("StoreCenter", "product exists");
                StockListItem stockListItem = DataLayerInstance.GetProductFromStore(_storeName, productName);
                handleIfLottery(stockListItem);
                DataLayerInstance.RemoveStockListItem(stockListItem);
                answer = new StoreAnswer(StoreEnum.Success, "product removed");
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
        }

        private void handleIfLottery(StockListItem stockListItem)
        {
            if (stockListItem.PurchaseWay == PurchaseEnum.Lottery)
            {
                LotterySaleManagmentTicket LotteryManagment = DataLayerInstance.GetLotteryByProductID(stockListItem.Product.SystemId);
                LotteryManagment.InformCancel(syncher);
                DataLayerInstance.RemoveLottery(LotteryManagment);
            }
        }


    }
}