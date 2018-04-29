using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class RemoveDiscountFromProductSlave : AbstractSlave
    {
        internal MarketAnswer answer;

        public RemoveDiscountFromProductSlave(string storeName, IUserSeller storeManager) :base(storeName,storeManager)
        {
        }

        internal void RemoveDiscountFromProduct(string productName)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to remove discount from product in store");
                MarketLog.Log("StoreCenter", "check if store exists");
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " check if has premmision to edit products");
                _storeManager.CanDeclareDiscountPolicy();
                MarketLog.Log("StoreCenter", " has premmission");
                Product P = global.getProductByNameFromStore(_storeName, productName);
                checkifProductExists(P);
                Discount D = checkIfDiscountExistsPrivateMethod(productName);
                StockListItem stockListItem = global.GetProductFromStore(_storeName, productName);
                stockListItem.Discount = null;
                global.RemoveDiscount(D);
                global.EditStockInDatabase(stockListItem);
                MarketLog.Log("StoreCenter", "discount removed successfully");
                answer = new StoreAnswer(DiscountStatus.Success, "discount removed successfully");
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
        private Discount checkIfDiscountExistsPrivateMethod(string productName)
        {
            StockListItem stockListItem = global.GetProductFromStore(_storeName, productName);
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