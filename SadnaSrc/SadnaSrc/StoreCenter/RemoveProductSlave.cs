using System;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class RemoveProductSlave :AbstractStoreCenterSlave
    {
        public MarketAnswer Answer;
        IOrderSyncher syncher;

        public RemoveProductSlave(IOrderSyncher _syncher, string name, IUserSeller manager, IStoreDL storeDL) : base(name, manager, storeDL)
        {
            syncher = _syncher;
        }

        public void RemoveProduct(string productName)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to remove product from store");
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " check if has premmision to remove products");
                _storeManager.CanManageProducts();
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
				Answer = new StoreAnswer((StoreEnum)exe.Status, exe.GetErrorMessage());
			}
            catch (DataException e)
            {
                Answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
            }
            catch (MarketException)
            {
                Answer = new StoreAnswer(StoreEnum.NoPermission, "you have no premmision to do that");
            }
        }

        private void HandleIfLottery(StockListItem stockListItem)
        {
            if (stockListItem.PurchaseWay != PurchaseEnum.Lottery) return;
            LotterySaleManagmentTicket lotteryManagment = DataLayerInstance.GetLotteryByProductID(stockListItem.Product.SystemId);
            lotteryManagment.InformCancel(syncher);
            DataLayerInstance.RemoveLottery(lotteryManagment);
        }


    }
}