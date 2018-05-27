using System;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class AddCategoryDiscountSlave : AbstractStoreCenterSlave
    {
        public MarketAnswer Answer { get; set; }
        public AddCategoryDiscountSlave(string storeName, IUserSeller storeManager, IStoreDL storeDl) : base(storeName, storeManager, storeDl)
        {
        }
        internal void AddCategoryDiscount(string categoryName, DateTime startDate, DateTime endDate, int discountAmount)
        {

            try
            {
                MarketLog.Log("StoreCenter", "trying to add discount to category in store");
                MarketLog.Log("StoreCenter", "check if store exists");
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to edit categorys");
                _storeManager.CanDeclareDiscountPolicy();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if Category exists");
                CheckIfCategoryExists(categoryName);
                MarketLog.Log("StoreCenter", "check if dates are OK");
                CheckIfDatesOK(startDate, endDate);
                MarketLog.Log("StoreCenter", "check that discount amount is OK");
                CheckAmountIsOk(discountAmount);
                MarketLog.Log("StoreCenter", "check that the category don't have another discount");
                CheckHasNoExistsDiscount(categoryName);
                CategoryDiscount discount =
                    new CategoryDiscount(categoryName, _storeName, startDate, endDate, discountAmount);
                DataLayerInstance.AddCategoryDiscount(discount);
                MarketLog.Log("StoreCenter", "categoryDiscountd added successfully");
                string[] coupon = {discount.SystemId};
                Answer = new StoreAnswer(StoreEnum.Success, "categoryDiscountd added successfully", coupon);
        }
        catch (StoreException exe)
        {
            Answer = new StoreAnswer((StoreEnum) exe.Status, exe.GetErrorMessage());
        }
        catch (MarketException)
        {
            Answer = new StoreAnswer(StoreEnum.NoPermission, "you have no premmision to do that");
        }
        catch (DataException e)
        {
            Answer = new StoreAnswer((StoreEnum) e.Status, e.GetErrorMessage());
        }
     }

        private void CheckIfCategoryExists(string categoryName)
        {
            Category category = DataLayerInstance.GetCategoryByName(categoryName);
            if (category == null)
            {
                throw new StoreException(StoreEnum.CategoryNotExistsInSystem, "category exists in the store");
            }
        }
        private static void CheckAmountIsOk(int discountAmount)
        {
            if (discountAmount >= 100)
            {
                MarketLog.Log("StoreCenter", "discount amount is >=100%");
                throw new StoreException(DiscountStatus.AmountIsHundredAndpresenteges, "DiscountAmount is >= 100%");
            }
            if (discountAmount <= 0)
            {
                MarketLog.Log("StoreCenter", "discount amount <=0");
                throw new StoreException(DiscountStatus.DiscountAmountIsNegativeOrZero, "DiscountAmount is >= 100%");
            }
        }

        private void CheckHasNoExistsDiscount(string categoryName)
        {
            CategoryDiscount categoryDiscount = DataLayerInstance.GetCategoryDiscount(categoryName, _storeName);
            if (categoryDiscount != null)
            {
                throw new StoreException(StoreEnum.CategoryDiscountAlreadyExistsInStore, "category discount exists in the store");
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