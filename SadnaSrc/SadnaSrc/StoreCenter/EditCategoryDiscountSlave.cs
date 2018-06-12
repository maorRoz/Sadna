using System;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class EditCategoryDiscountSlave : AbstractStoreCenterSlave
    {
        public MarketAnswer Answer { get; set; }
        public EditCategoryDiscountSlave(string storeName, IUserSeller storeManager, IStoreDL storeDl) : base(storeName, storeManager, storeDl)
        {
        }

        public void EditCategoryDiscount(string categoryName, string whatToEdit, string newValue)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to edit discount from product in store");
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " check if has premmision to edit products");
                _storeManager.CanDeclareDiscountPolicy();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check that cateory exists");
                CheckIfCategoryExists(categoryName);
                MarketLog.Log("StoreCenter", "category exists");
                MarketLog.Log("StoreCenter", " check that category has discount in this store");
                CategoryDiscount categoryDiscount = CheckHasExistsCategoryDiscount(categoryName);
                switch (whatToEdit)
                {
                    case "startDate":
                    case "start Date":
                    case "StartDate":
                    case "Start Date":
                    case "startdate":
                    case "start date":
                    case "STARTDATE":
                    case "START DATE":
                        categoryDiscount = EditDiscountStartDatePrivateMethod(categoryDiscount, newValue);
                        break;
                    case "EndDate":
                    case "end Date":
                    case "enddate":
                    case "End Date":
                    case "end date":
                    case "ENDDATE":
                    case "END DATE":
                        categoryDiscount = EditDiscountEndDatePrivateMethod(categoryDiscount, newValue);
                        break;
                    case "DiscountAmount":
                    case "Discount Amount":
                    case "discount amount":
                    case "discountamount":
                    case "DISCOUNTAMOUNT":
                    case "DISCOUNT AMOUNT":
                        categoryDiscount = EditDiscountDiscountAmountPrivateMehtod(categoryDiscount, newValue);
                        break;
                }

                    if (Answer == null) { throw new StoreException(DiscountStatus.NoLegalAttrebute, "no legal attribute found"); }
                    DataLayerInstance.EditCategoryDiscount(categoryDiscount);
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
            private CategoryDiscount EditDiscountStartDatePrivateMethod(CategoryDiscount categoryDiscount, string newValue)
            {
                MarketLog.Log("StoreCenter", " edit start date");
                MarketLog.Log("StoreCenter", " checking that the start date is legal");
                if (!DateTime.TryParse(newValue, out var startTime))
                {
                    MarketLog.Log("StoreCenter", "date format is not legal");
                    throw new StoreException(DiscountStatus.DatesAreWrong, "date format is not legal");
                }
                if (startTime.Date < MarketYard.MarketDate)
                {
                    MarketLog.Log("StoreCenter", "can't set start time in the past");
                    throw new StoreException(DiscountStatus.DatesAreWrong, "can't set start time in the past");
                }

                if (startTime.Date >= categoryDiscount.EndDate.Date)
                {
                    MarketLog.Log("StoreCenter", "can't set start time that is later then the discount end time");
                    throw new StoreException(DiscountStatus.DatesAreWrong, "can't set start time that is later then the discount end time");
                }
                categoryDiscount.StartDate = startTime;
                MarketLog.Log("StoreCenter", " start date changed successfully");
                Answer = new StoreAnswer(DiscountStatus.Success, "category " + categoryDiscount.CategoryName + " Start Date become " + startTime);
                return categoryDiscount;
            }
            private CategoryDiscount EditDiscountDiscountAmountPrivateMehtod(CategoryDiscount categoryDiscount, string newValue)
            {

                MarketLog.Log("StoreCenter", " edit discount amount");
                if (!Int32.TryParse(newValue, out var newintValue))
                {
                    MarketLog.Log("StoreCenter", "value is not legal");
                    throw new StoreException(DiscountStatus.DiscountAmountIsNotNumber, "value is not legal");
                }
                if (newintValue >= 100)
                {
                    MarketLog.Log("StoreCenter", "DiscountAmount is >= 100");
                    throw new StoreException(DiscountStatus.AmountIsHundredAndpresenteges, "DiscountAmount is >= 100");
                }
                if (newintValue <= 0)
                {
                    MarketLog.Log("StoreCenter", "discount amount <=0");
                    throw new StoreException(DiscountStatus.DiscountAmountIsNegativeOrZero, "DiscountAmount is >= 100%");
                }
                categoryDiscount.DiscountAmount = newintValue;
                MarketLog.Log("StoreCenter", "discount amount set to " + newintValue);
                Answer = new StoreAnswer(DiscountStatus.Success, "category " + categoryDiscount.CategoryName + "discount amount become " + newValue);
                return categoryDiscount;
            }
            private CategoryDiscount EditDiscountEndDatePrivateMethod(CategoryDiscount categoryDiscount, string newValue)
            {

                MarketLog.Log("StoreCenter", " edit start date");
                MarketLog.Log("StoreCenter", " checking that the start date is legal");
                if (!DateTime.TryParse(newValue, out var endDate))
                {
                    MarketLog.Log("StoreCenter", "date format is not legal");
                    throw new StoreException(DiscountStatus.DatesAreWrong, "date format is not legal");
                }
                if (endDate.Date < MarketYard.MarketDate)
                {
                    MarketLog.Log("StoreCenter", "can't set end time in the past");
                    throw new StoreException(DiscountStatus.DatesAreWrong, "can't set end time in the past");
                }
                if (endDate.Date < categoryDiscount.StartDate.Date)
                {
                    MarketLog.Log("StoreCenter", "can't set end time that is sooner then the discount start time");
                    throw new StoreException(DiscountStatus.DatesAreWrong, "can't set end time that is sooner then the discount start time");
                }
                categoryDiscount.EndDate = endDate;
                MarketLog.Log("StoreCenter", " start date changed successfully");
                Answer = new StoreAnswer(StoreEnum.Success, "category " + categoryDiscount.CategoryName + " discount End Date become " + endDate);
                return categoryDiscount;
            }
            private void CheckIfCategoryExists(string categoryName)
            {
                Category category = DataLayerInstance.GetCategoryByName(categoryName);
                if (category == null)
                {
                    throw new StoreException(StoreEnum.CategoryNotExistsInSystem, "category not exists in the store");
                }
            }
            private CategoryDiscount CheckHasExistsCategoryDiscount(string categoryName)
            {
                CategoryDiscount categoryDiscount = DataLayerInstance.GetCategoryDiscount(categoryName, _storeName);
                if (categoryDiscount == null)
                {
                    throw new StoreException(StoreEnum.CategoryDiscountNotExistsInStore, "categorydiscount not exists in the store");
                }
                return categoryDiscount;
            }
        }
    }