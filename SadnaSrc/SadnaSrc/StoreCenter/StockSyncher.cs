using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using System;
using System.Collections.Generic;
using SadnaSrc.MarketFeed;

namespace SadnaSrc.StoreCenter
{
    public class StockSyncher : IStockSyncher
    {
        private static StockSyncher instance;

        public IStoreDL DataLayer { get; }

        public static StockSyncher Instance => instance ?? (instance = new StockSyncher());
        private StockSyncher()
        {
            DataLayer = StoreDL.Instance;
        }
        
        public StockListItem GetProductFromStore(string store, string productName)
        {
            CheckThatStoreExitst(store);
            CheckThatProductExitst(store, productName);
            return DataLayer.GetProductFromStore(store, productName);
        }

        

        public void UpdateQuantityAfterPurchase(string storeName, string productName, int quantity)
        {
            CheckThatStoreExitst(storeName);
            CheckThatProductExitst(storeName, productName);
            StockListItem product = DataLayer.GetProductFromStore(storeName, productName);
            if (product.Quantity < quantity || quantity <= 0)
            { throw new StoreException(StoreSyncStatus.NoProduct, "product doesn't exist in this quantity"); }
            product.Quantity -= quantity;
            DataLayer.EditStockInDatabase(product);
        }

        public bool ProductExistsInQuantity(string storeName, string product, int quantity)
        {
            CheckThatStoreExitst(storeName);
            CheckThatProductExitst(storeName, product);
            StockListItem sli = DataLayer.GetProductFromStore(storeName, product);
            return sli != null && sli.Quantity >= quantity;
        }
        
      
        public void UpdateLottery(string storeName, string productName, double moenyPayed, string UserName, IOrderSyncher syncher, int cheatCode)
        {
            LotterySaleManagmentTicket lotto = DataLayer.GetLotteryByProductNameAndStore(storeName, productName);
            if (!lotto.updateLottery(moenyPayed, DataLayer.GetUserIDFromUserName(UserName))) return;
            syncher.CloseLottery(lotto.Original.Name, lotto.storeName, lotto.getWinnerID(cheatCode));
            UpdateQuantityAfterPurchase(storeName,productName,1);
            Publisher.Instance.NotifyLotteryFinish(lotto.SystemID,storeName, productName);
        }
        // this fucntion calculate item price if it has Hidden discount. happend only in Purches time and this is way it's happening here
        public double CalculateItemPriceWithDiscount(string storeName, string productName, string _DiscountCode, int _quantity)
        {
            if (!DataLayer.IsStoreExistAndActive(storeName))
                throw new StoreException(CalculateEnum.StoreNotExists, "store not exists");
            if (IsProductNameAvailableInStore(storeName, productName))
                throw new StoreException(CalculateEnum.ProductNotFound, "Product Not Found");
            StockListItem item = DataLayer.GetProductFromStore(storeName, productName);
            if (_quantity > item.Quantity)
                throw new StoreException(CalculateEnum.QuantityIsGreaterThenStack, "quantity Is Greater Then Stack");
            if (_quantity <= 0)
                throw new StoreException(CalculateEnum.QuanitityIsNonPositive, "quanitity is <=0");
            if (item.Discount == null)
                throw new StoreException(CalculateEnum.ProductHasNoDiscount, "product has no discount");
            if (item.Discount.discountCode != _DiscountCode)
                throw new StoreException(CalculateEnum.DiscountCodeIsWrong, "discount code is wrong");
            if (item.Discount.discountType != DiscountTypeEnum.Hidden)
                throw new StoreException(CalculateEnum.DiscountIsNotHidden, "discount Is Not Hiddeng");
            if (MarketYard.MarketDate < item.Discount.startDate.Date)
                throw new StoreException(CalculateEnum.DiscountNotStarted, "Discount Time Not Started Yet");
            if (MarketYard.MarketDate > item.Discount.EndDate.Date)
                throw new StoreException(CalculateEnum.DiscountExpired, "discount expired");
            double ans = item.Discount.CalcDiscount(item.Product.BasePrice);
            ans = ans * _quantity;
            return ans;
        }

        public bool HasActiveLottery(string storeName, string productName, double priceWantToPay)
        {
            LotterySaleManagmentTicket lotto;
            StockListItem item;
            try
            {
                item = DataLayer.GetProductFromStore(storeName, productName);
            }
            catch (Exception)
            { return false; }

            if (item?.PurchaseWay != PurchaseEnum.Lottery)
                return false;
            try
            {
                lotto = DataLayer.GetLotteryByProductNameAndStore(storeName, productName);
            }
            catch (Exception)
            { return false; }
            if (lotto == null)
                return false;
            if (!lotto.IsActive)
                return false;
            if (priceWantToPay <= 0)
                return false;
            return lotto.CanPurchase(priceWantToPay) && lotto.CheckDatesWhenPurches();
        }

        
        private bool IsProductNameAvailableInStore(string storeName, string productName)
        {
            var product = DataLayer.GetProductByNameFromStore(storeName, productName);
            return product == null;
        }
        private void CheckThatProductExitst(string storeName, string product)
        {
            var prod = DataLayer.GetProductByNameFromStore(storeName, product);
            if (prod == null)
            { throw new StoreException(StoreEnum.ProductNotFound, "product not exists in store"); }
        }

        private void CheckThatStoreExitst(string storeName)
        {
            Store store = DataLayer.GetStorebyName(storeName);
            if (store == null)
            { throw new StoreException(StoreEnum.StoreNotExists, "store not exists"); }
        }
    }
}