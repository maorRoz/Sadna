using System;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class RemoveDiscountFromProductSlave : AbstractStoreCenterSlave
    {
        public MarketAnswer Answer;

        public RemoveDiscountFromProductSlave(string storeName, IUserSeller storeManager, IStoreDL storeDL) : base(storeName, storeManager, storeDL)
        {
        }

        public void RemoveDiscountFromProduct(string productName)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to remove discount from product in store");
                MarketLog.Log("StoreCenter", "check if store exists");
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " check if has premmision to edit products");
                _storeManager.CanDeclareDiscountPolicy();
                MarketLog.Log("StoreCenter", " has premmission");
                Product P = DataLayerInstance.GetProductByNameFromStore(_storeName, productName);
                checkifProductExists(P);
                Discount D = CheckIfDiscountExistsPrivateMethod(productName);
                StockListItem stockListItem = DataLayerInstance.GetProductFromStore(_storeName, productName);
                stockListItem.Discount = null;
                DataLayerInstance.RemoveDiscount(D);
                DataLayerInstance.EditStockInDatabase(stockListItem);
                MarketLog.Log("StoreCenter", "discount removed successfully");
                Answer = new StoreAnswer(DiscountStatus.Success, "discount removed successfully");
            }
            catch (StoreException exe)
            {
                Answer = new StoreAnswer((StoreEnum)exe.Status,exe.GetErrorMessage());
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
        private Discount CheckIfDiscountExistsPrivateMethod(string productName)
        {
            StockListItem stockListItem = DataLayerInstance.GetProductFromStore(_storeName, productName);
            MarketLog.Log("StoreCenter", " Product exists");
            MarketLog.Log("StoreCenter", "checking that the product has a discount");
            Discount discount = stockListItem.Discount;
            if (discount == null)
            {
                MarketLog.Log("StoreCenter", "product does not exists");
                throw new StoreException(DiscountStatus.DiscountNotFound, "there is no discount at this product");
            }
            MarketLog.Log("StoreCenter", " check what you want to edit");
            return discount;
        }
    }
}