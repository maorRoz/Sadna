using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class AddDiscountToProductSlave : AbstractSlave
    {
        internal MarketAnswer answer;
        private All_ID_Manager ID_Manager;
        public AddDiscountToProductSlave(string storeName, IUserSeller storeManager) :base(storeName,storeManager)
        {
            ID_Manager = All_ID_Manager.GetInstance();
        }

        internal Discount AddDiscountToProduct(string productName, DateTime startDate, DateTime endDate, int discountAmount, string discountType, bool presenteges)
        {
            MarketLog.Log("StoreCenter", "trying to add discount to product in store");
            MarketLog.Log("StoreCenter", "check if store exists");
            try
            {
                checkIfStoreExistsAndActiveDiscount();
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to edit products");
                _storeManager.CanDeclareDiscountPolicy();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if product name exists in the store " + _storeName);
                Product product = global.getProductByNameFromStore(_storeName, productName);
                checkifProductExistsDiscount(product);
                MarketLog.Log("StoreCenter", "check if dates are OK");
                checkIfDatesOK(startDate,endDate);
                MarketLog.Log("StoreCenter", "check that discount amount is OK");
                checkPresentegesAndAmountOK(discountAmount, presenteges, product);
                StockListItem stockListItem = global.GetProductFromStore(_storeName, product.Name);
                MarketLog.Log("StoreCenter", "check that the product don't have another discount");
                checkHasNoExistsDiscount(stockListItem);
                string dicoundCode = ID_Manager.GetDiscountCode();
                Discount discount = new Discount(dicoundCode, EnumStringConverter.GetdiscountTypeEnumString(discountType), startDate,
                    endDate, discountAmount, presenteges);
                stockListItem.Discount = discount;
                global.AddDiscount(discount);
                global.EditStockInDatabase(stockListItem);
                MarketLog.Log("StoreCenter", "discount added successfully");
                string[] coupon = { dicoundCode };
                answer = new StoreAnswer(DiscountStatus.Success, "discount added successfully", coupon);
                return discount;
            }
            catch (StoreException exe)
            {
                answer = new StoreAnswer(exe);
                return null;
            }
            catch (MarketException)
            {
                answer = new StoreAnswer(StoreEnum.NoPremmision, "you have no premmision to do that");
                return null;
            }
        }
        protected void checkIfStoreExistsAndActiveDiscount()
        {
            if (!global.IsStoreExistAndActive(_storeName))
            { throw new StoreException(DiscountStatus.NoStore, "store not exists or active"); }
        }
        protected void checkifProductExistsDiscount(Product product)
        {
            if (product == null)
            {
                MarketLog.Log("StoreCenter", "product not exists");
                throw new StoreException(DiscountStatus.ProductNotFound, "no Such Product");
            }
            MarketLog.Log("StoreCenter", "product exists");
        }
        private void checkHasNoExistsDiscount(StockListItem stockListItem)
        {
            if (stockListItem.Discount != null)
            {
                MarketLog.Log("StoreCenter", "the product have another discount");
                throw new StoreException(DiscountStatus.thereIsAlreadyAnotherDiscount, "the product have another discount");
            }
        }

        private void checkPresentegesAndAmountOK(int discountAmount, bool presenteges, Product product)
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
                throw new StoreException(DiscountStatus.discountAmountIsNegativeOrZero, "DiscountAmount is >= 100%");
            }
        }

        private void checkIfDatesOK(DateTime startDate, DateTime endDate)
        {
            if ((startDate < MarketYard.MarketDate) || !(startDate < endDate))
            {
                MarketLog.Log("StoreCenter", "something wrong with the dates");
                throw new StoreException(DiscountStatus.DatesAreWrong, "dates are not leagal");
            }
        }
    }
}