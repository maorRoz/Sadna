using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class RemoveProductSlave :AbstractStoreCenterSlave
    {
        public MarketAnswer Answer;
        IOrderSyncher syncher;

        public RemoveProductSlave(ref IOrderSyncher _syncher, string name, IUserSeller manager, I_StoreDL storeDL) : base(name, manager, storeDL)
        {
            syncher = _syncher;
        }

        public void RemoveProduct(string productName)
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
                Product product = DataLayerInstance.GetProductByNameFromStore(_storeName, productName);
                checkifProductExists(product);
                MarketLog.Log("StoreCenter", "product exists");
                StockListItem stockListItem = DataLayerInstance.GetProductFromStore(_storeName, productName);
                HandleIfLottery(stockListItem);
                DataLayerInstance.RemoveStockListItem(stockListItem);
                Answer = new StoreAnswer(StoreEnum.Success, "product removed");
            }
            catch (StoreException exe)
            {
                Answer = new StoreAnswer(exe);
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                Answer = new StoreAnswer(StoreEnum.NoPremmision, "you have no premmision to do that");
            }
        }

        private void HandleIfLottery(StockListItem stockListItem)
        {
            if (stockListItem.PurchaseWay == PurchaseEnum.Lottery)
            {
                LotterySaleManagmentTicket lotteryManagment = DataLayerInstance.GetLotteryByProductID(stockListItem.Product.SystemId);
                lotteryManagment.InformCancel(syncher);
                DataLayerInstance.RemoveLottery(lotteryManagment);
            }
        }


    }
}