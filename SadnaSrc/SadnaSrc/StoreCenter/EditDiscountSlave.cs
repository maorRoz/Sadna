using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class EditDiscountSlave : AbstractStoreCenterSlave
    {
        internal MarketAnswer answer;

        public EditDiscountSlave(string storeName, IUserSeller storeManager) : base(storeName, storeManager)
        {
        }

        internal void EditDiscount(string productName, string whatToEdit, string newValue)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to edit discount from product in store");
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " check if has premmision to edit products");
                _storeManager.CanDeclareDiscountPolicy();
                MarketLog.Log("StoreCenter", " has premmission");
                IsProductNameAvailableInStore(productName);
                Discount discount = checkIfDiscountExistsPrivateMethod(productName);
                if ((whatToEdit == "discountType") || (whatToEdit == "DiscountType") || (whatToEdit == "discounttype") || (whatToEdit == "DISCOUNTTYPE"))
                {
                    discount = editDiscountdiscyoutTypePrivateMethod(discount, newValue, productName);

                }
                if ((whatToEdit == "startDate") || (whatToEdit == "start Date") || (whatToEdit == "StartDate") || (whatToEdit == "Start Date") || (whatToEdit == "startdate") || (whatToEdit == "start date") || (whatToEdit == "STARTDATE") || (whatToEdit == "START DATE"))
                {
                    discount = editDiscountStartDatePrivateMethod(discount, newValue, productName);
                }

                if ((whatToEdit == "EndDate") || (whatToEdit == "end Date") || (whatToEdit == "enddate") || (whatToEdit == "End Date") || (whatToEdit == "end date") || (whatToEdit == "ENDDATE") || (whatToEdit == "END DATE"))
                {
                    discount = editDiscountEndDatePrivateMethod(discount, newValue, productName);
                }

                if ((whatToEdit == "DiscountAmount") || (whatToEdit == "Discount Amount") || (whatToEdit == "discount amount") || (whatToEdit == "discountamount") || (whatToEdit == "DISCOUNTAMOUNT") || (whatToEdit == "DISCOUNT AMOUNT"))
                {
                    discount = editDiscountDiscountAmountPrivateMehtod(discount, newValue, productName);
                }
                if ((whatToEdit == "Percentages") || (whatToEdit == "percentages") || (whatToEdit == "PERCENTAGES"))
                {
                    discount = editDiscountPercentagesPrivateMehtod(discount, newValue, productName);
                }
                if (answer == null) { throw new StoreException(DiscountStatus.NoLegalAttrebute, "no leagal attrebute found"); }
                DataLayerInstance.EditDiscountInDatabase(discount);
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
        private Discount editDiscountdiscyoutTypePrivateMethod(Discount discount, string newValue, string productName)
        {
            MarketLog.Log("StoreCenter", " edit discount type");
            discount.discountType = EnumStringConverter.GetdiscountTypeEnumString(newValue);
            MarketLog.Log("StoreCenter", " discount type changed successfully");
            answer = new StoreAnswer(StoreEnum.Success, "item " + productName + " discount type become " + newValue);
            return discount;
        }
        private Discount editDiscountStartDatePrivateMethod(Discount discount, string newValue, string productName)
        {

            MarketLog.Log("StoreCenter", " edit start date");
            MarketLog.Log("StoreCenter", " checking that the start date is legal");
            DateTime startTime = DateTime.MaxValue;
            if (!DateTime.TryParse(newValue, out startTime))
            {
                MarketLog.Log("StoreCenter", "date format is not legal");
                throw new StoreException(DiscountStatus.DatesAreWrong, "date format is not legal");
            }
            if (startTime.Date < MarketYard.MarketDate)
            {
                MarketLog.Log("StoreCenter", "can't set start time in the past");
                throw new StoreException(DiscountStatus.DatesAreWrong, "can't set start time in the past");
            }

            if (startTime.Date >= discount.EndDate.Date)
            {
                MarketLog.Log("StoreCenter", "can't set start time that is later then the discount end time");
                throw new StoreException(DiscountStatus.DatesAreWrong, "can't set start time that is later then the discount end time");
            }
            discount.startDate = startTime;
            MarketLog.Log("StoreCenter", " start date changed successfully");
            answer = new StoreAnswer(StoreEnum.Success, "item " + productName + " discount Start Date become " + startTime);
            return discount;
        }
        private Discount editDiscountPercentagesPrivateMehtod(Discount discount, string newValue, string productName)
        {
            MarketLog.Log("StoreCenter", "try to edit precenteges");
            bool newboolValue = true;
            if (!Boolean.TryParse(newValue, out newboolValue))
            {
                MarketLog.Log("StoreCenter", "value is not legal");
                throw new StoreException(DiscountStatus.precentegesIsNotBoolean, "value is not legal");
            }
            MarketLog.Log("StoreCenter", "checking that the discount amount is fit to precenteges");
            if ((newboolValue) && (discount.DiscountAmount >= 100))
            {
                MarketLog.Log("StoreCenter", "DiscountAmount is >= 100, cant make it presenteges");
                throw new StoreException(DiscountStatus.AmountIsHundredAndpresenteges, "DiscountAmount is >= 100, cant make it presenteges");
            }
            discount.Percentages = newboolValue;
            if (newboolValue)
                answer = new StoreAnswer(StoreEnum.Success, "item " + productName + " discount preseneges become true");
            answer = new StoreAnswer(StoreEnum.Success, "item " + productName + " discount preseneges become false");
            return discount;
        }
        private Discount editDiscountDiscountAmountPrivateMehtod(Discount discount, string newValue, string productName)
        {

            MarketLog.Log("StoreCenter", " edit discount amount");
            int newintValue = 0;
            if (!Int32.TryParse(newValue, out newintValue))
            {
                MarketLog.Log("StoreCenter", "value is not legal");
                throw new StoreException(DiscountStatus.discountAmountIsNotNumber, "value is not legal");
            }
            if ((discount.Percentages) && (newintValue >= 100))
            {
                MarketLog.Log("StoreCenter", "DiscountAmount is >= 100, cant make it presenteges");
                throw new StoreException(DiscountStatus.AmountIsHundredAndpresenteges, "DiscountAmount is >= 100, cant make it presenteges");
            }
            if ((!discount.Percentages) && (newintValue > DataLayerInstance.getProductByNameFromStore(_storeName, productName).BasePrice))
            {
                MarketLog.Log("StoreCenter", "discount amount is >= product price");
                throw new StoreException(DiscountStatus.DiscountGreaterThenProductPrice, "DiscountAmount is > then product price");
            }
            if (newintValue <= 0)
            {
                MarketLog.Log("StoreCenter", "discount amount <=0");
                throw new StoreException(DiscountStatus.discountAmountIsNegativeOrZero, "DiscountAmount is >= 100%");
            }
            discount.DiscountAmount = newintValue;
            MarketLog.Log("StoreCenter", "discount amount set to " + newintValue);
            answer = new StoreAnswer(StoreEnum.Success, "item " + productName + " discount amount become " + newValue);
            return discount;
        }
        private Discount editDiscountEndDatePrivateMethod(Discount discount, string newValue, string productName)
        {

            MarketLog.Log("StoreCenter", " edit start date");
            MarketLog.Log("StoreCenter", " checking that the start date is legal");
            DateTime EndDate = DateTime.MaxValue;
            if (!DateTime.TryParse(newValue, out EndDate))
            {
                MarketLog.Log("StoreCenter", "date format is not legal");
                throw new StoreException(DiscountStatus.DatesAreWrong, "date format is not legal");
            }
            if (EndDate.Date < MarketYard.MarketDate)
            {
                MarketLog.Log("StoreCenter", "can't set end time in the past");
                throw new StoreException(DiscountStatus.DatesAreWrong, "can't set end time in the past");
            }
            if (EndDate.Date < discount.startDate.Date)
            {
                MarketLog.Log("StoreCenter", "can't set end time that is sooner then the discount start time");
                throw new StoreException(DiscountStatus.DatesAreWrong, "can't set end time that is sooner then the discount start time");
            }
            discount.EndDate = EndDate;
            MarketLog.Log("StoreCenter", " start date changed successfully");
            answer = new StoreAnswer(StoreEnum.Success, "item " + productName + " discount End Date become " + EndDate);
            return discount;
        }
    }
}