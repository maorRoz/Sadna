using System;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class AddDiscountToProductSlave : AbstractStoreCenterSlave
    {
        public MarketAnswer answer;
        public AddDiscountToProductSlave(string storeName, IUserSeller storeManager, IStoreDL storeDL) :base(storeName,storeManager, storeDL)
        {
        }

        public Discount AddDiscountToProduct(string productName, DateTime startDate, DateTime endDate,
            int discountAmount, string discountType, bool presenteges)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to add discount to product in store");
                MarketLog.Log("StoreCenter", "check if store exists");
                CheckIfStoreExistsAndActiveDiscount();
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to edit products");
                _storeManager.CanDeclareDiscountPolicy();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if product name exists in the store " + _storeName);
                Product product = DataLayerInstance.GetProductByNameFromStore(_storeName, productName);
                CheckifProductExistsDiscount(product);
                MarketLog.Log("StoreCenter", "check if dates are OK");
                CheckIfDatesOK(startDate,endDate);
                MarketLog.Log("StoreCenter", "check that discount amount is OK");
                CheckPresentegesAndAmountOK(discountAmount, presenteges, product);
                StockListItem stockListItem = DataLayerInstance.GetProductFromStore(_storeName, product.Name);
                MarketLog.Log("StoreCenter", "check that the product don't have another discount");
                CheckHasNoExistsDiscount(stockListItem);
                Discount discount = new Discount(EnumStringConverter.GetdiscountTypeEnumString(discountType), startDate,
                    endDate, discountAmount, presenteges);
                stockListItem.Discount = discount;
                DataLayerInstance.AddDiscount(discount);
                DataLayerInstance.EditStockInDatabase(stockListItem);
                MarketLog.Log("StoreCenter", "discount added successfully");
                string[] coupon = { discount.discountCode};
                answer = new StoreAnswer(DiscountStatus.Success, "discount added successfully", coupon);
                return discount;
            }
            catch (StoreException exe)
            {
                answer = new StoreAnswer((StoreEnum)exe.Status, exe.GetErrorMessage());
            }
            catch (MarketException)
            {
                answer = new StoreAnswer(StoreEnum.NoPermission, "you have no premmision to do that");
            }
            catch (DataException e)
            {
                answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
            }

            return null;
        }
        private void CheckIfStoreExistsAndActiveDiscount()
        {
            if (!DataLayerInstance.IsStoreExistAndActive(_storeName))
            { throw new StoreException(DiscountStatus.NoStore, "store not exists or active"); }
        }
        private static void CheckifProductExistsDiscount(Product product)
        {
            if (product == null)
            {
                MarketLog.Log("StoreCenter", "product not exists");
                throw new StoreException(DiscountStatus.ProductNotFound, "no Such Product");
            }
            MarketLog.Log("StoreCenter", "product exists");
        }
        private static void CheckHasNoExistsDiscount(StockListItem stockListItem)
        {
            if (stockListItem.Discount == null) return;
            MarketLog.Log("StoreCenter", "the product have another discount");
            throw new StoreException(DiscountStatus.ThereIsAlreadyAnotherDiscount, "the product have another discount");
        }

        private static void CheckPresentegesAndAmountOK(int discountAmount, bool presenteges, Product product)
        {
            if (presenteges && (discountAmount >= 100))
            {
                MarketLog.Log("StoreCenter", "discount amount is >=100%");
                throw new StoreException(DiscountStatus.AmountIsHundredAndpresenteges, "DiscountAmount is >= 100%");
            }
            if (!presenteges && (discountAmount > product.BasePrice))
            {
                MarketLog.Log("StoreCenter", "discount amount is >= product price");
                throw new StoreException(DiscountStatus.DiscountGreaterThenProductPrice, "DiscountAmount is > then product price");
            }
            if (discountAmount <= 0)
            {
                MarketLog.Log("StoreCenter", "discount amount <=0");
                throw new StoreException(DiscountStatus.DiscountAmountIsNegativeOrZero, "DiscountAmount is >= 100%");
            }
        }

        private void CheckIfDatesOK(DateTime startDate, DateTime endDate)
        {
            if (startDate >= MarketYard.MarketDate && startDate < endDate) return;
            MarketLog.Log("StoreCenter", "something wrong with the dates");
            throw new StoreException(DiscountStatus.DatesAreWrong, "dates are not leagal");
        }
    }
}