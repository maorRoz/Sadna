using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using System;
using System.Collections.Generic;

namespace SadnaSrc.StoreCenter
{
    public class StoreSyncerImplementation : OutsideModuleService
    {
        static StoreSyncerImplementation instance;
        public I_StoreDL DataLayer { get; }
        public static StoreSyncerImplementation GetInstance()
        {
            if (instance == null)
            {
                instance = new StoreSyncerImplementation();
                return instance;
            }
            return instance;
        }
        private StoreSyncerImplementation()
        {
            DataLayer = StoreDL.GetInstance();
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
            if (sli!=null)
                return sli.Quantity >= quantity;
            return false;
        }
        
      
        public void updateLottery(string storeName, string ProductName, double moenyPayed, string UserName, IOrderSyncher syncher, int cheatCode)
        {
            LotterySaleManagmentTicket Lotto = DataLayer.GetLotteryByProductNameAndStore(storeName, ProductName);
            if (Lotto.updateLottery(moenyPayed, DataLayer.getUserIDFromUserName(UserName)))
            {
                syncher.CloseLottery(Lotto.Original.Name, Lotto.storeName, Lotto.getWinnerID(cheatCode));
            }
        }
        public double CalculateItemPriceWithDiscount(string storeName, string productName, string _DiscountCode, int _quantity)
        {
            if (!DataLayer.IsStoreExistAndActive(storeName))
                throw new StoreException(CalculateEnum.StoreNotExists, "store not exists");
            if (IsProductNameAvailableInStore(storeName, productName))
                throw new StoreException(CalculateEnum.ProductNotFound, "Product Not Found");
            StockListItem item = DataLayer.GetProductFromStore(storeName, productName);
            if (_quantity > item.Quantity)
                throw new StoreException(CalculateEnum.quantityIsGreaterThenStack, "quantity Is Greater Then Stack");
            if (_quantity <= 0)
                throw new StoreException(CalculateEnum.quanitityIsNonPositive, "quanitity is <=0");
            if (item.Discount == null)
                throw new StoreException(CalculateEnum.ProductHasNoDiscount, "product has no discount");
            if (item.Discount.discountCode != _DiscountCode)
                throw new StoreException(CalculateEnum.DiscountCodeIsWrong, "discount code is wrong");
            if (item.Discount.discountType != discountTypeEnum.Hidden)
                throw new StoreException(CalculateEnum.discountIsNotHidden, "discount Is Not Hiddeng");
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
            LotterySaleManagmentTicket Lotto;
            StockListItem item;
            try
            {
                item = DataLayer.GetProductFromStore(storeName, productName);
            }
            catch (Exception)
            { return false; }
            if (item == null)
                return false;
            if (item.PurchaseWay != PurchaseEnum.Lottery)
                return false;
            try
            {
                Lotto = DataLayer.GetLotteryByProductNameAndStore(storeName, productName);
            }
            catch (Exception)
            { return false; }
            if (Lotto == null)
                return false;
            if (!Lotto.IsActive)
                return false;
            if (priceWantToPay <= 0)
                return false;
            if (!Lotto.CanPurchase(priceWantToPay))
                return false;
            if (!Lotto.checkDatesWhenPurches())
                return false;
            return true;
        }

        
        private bool IsProductNameAvailableInStore(string storeName, string productName)
        {
            Product P = DataLayer.getProductByNameFromStore(storeName, productName);
            return (P == null);
        }
        private void CheckThatProductExitst(string storeName, string product)
        {
            Product P = DataLayer.getProductByNameFromStore(storeName, product);
            if (P == null)
            { throw new StoreException(StoreEnum.ProductNotFound, "product not exists in store"); }
        }

        private void CheckThatStoreExitst(string storeName)
        {
            Store S = DataLayer.getStorebyName(storeName);
            if (S == null)
            { throw new StoreException(StoreEnum.StoreNotExists, "store not exists"); }
        }
    }
}