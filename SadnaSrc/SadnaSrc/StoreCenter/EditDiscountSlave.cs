﻿using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class EditDiscountSlave : AbstractStoreCenterSlave
    {
        public MarketAnswer Answer;

        public EditDiscountSlave(string storeName, IUserSeller storeManager, IStoreDL storeDL) : base(storeName, storeManager, storeDL)
        {
        }

        public void EditDiscount(string productName, string whatToEdit, string newValue)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to edit discount from product in store");
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " check if has premmision to edit products");
                _storeManager.CanDeclareDiscountPolicy();
                MarketLog.Log("StoreCenter", " has premmission");
                IsProductNameAvailableInStore(productName);
                Discount discount = CheckIfDiscountExistsPrivateMethod(productName);
                switch (whatToEdit)
                {
                    case "discountType":
                    case "DiscountType":
                    case "discounttype":
                    case "DISCOUNTTYPE":
                        discount = EditDiscountdiscyoutTypePrivateMethod(discount, newValue, productName);
                        break;
                    case "startDate":
                    case "start Date":
                    case "StartDate":
                    case "Start Date":
                    case "startdate":
                    case "start date":
                    case "STARTDATE":
                    case "START DATE":
                        discount = EditDiscountStartDatePrivateMethod(discount, newValue, productName);
                        break;
                    case "EndDate":
                    case "end Date":
                    case "enddate":
                    case "End Date":
                    case "end date":
                    case "ENDDATE":
                    case "END DATE":
                        discount = EditDiscountEndDatePrivateMethod(discount, newValue, productName);
                        break;
                    case "DiscountAmount":
                    case "Discount Amount":
                    case "discount amount":
                    case "discountamount":
                    case "DISCOUNTAMOUNT":
                    case "DISCOUNT AMOUNT":
                        discount = EditDiscountDiscountAmountPrivateMehtod(discount, newValue, productName);
                        break;
                    case "Percentages":
                    case "percentages":
                    case "PERCENTAGES":
                        discount = EditDiscountPercentagesPrivateMehtod(discount, newValue, productName);
                        break;
                }

                if (Answer == null) { throw new StoreException(DiscountStatus.NoLegalAttrebute, "no legal attribute found"); }
                DataLayerInstance.EditDiscountInDatabase(discount);
            }
            catch (StoreException exe)
            {
                Answer = new StoreAnswer(exe);
            }
            catch (MarketException)
            {
                Answer = new StoreAnswer(StoreEnum.NoPremmision, "you have no premmision to do that");
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
        private Discount EditDiscountdiscyoutTypePrivateMethod(Discount discount, string newValue, string productName)
        {
            MarketLog.Log("StoreCenter", " edit discount type");
            discount.discountType = EnumStringConverter.GetdiscountTypeEnumString(newValue);
            MarketLog.Log("StoreCenter", " discount type changed successfully");
            Answer = new StoreAnswer(StoreEnum.Success, "item " + productName + " discount type become " + newValue);
            return discount;
        }
        private Discount EditDiscountStartDatePrivateMethod(Discount discount, string newValue, string productName)
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
            Answer = new StoreAnswer(StoreEnum.Success, "item " + productName + " discount Start Date become " + startTime);
            return discount;
        }
        private Discount EditDiscountPercentagesPrivateMehtod(Discount discount, string newValue, string productName)
        {
            MarketLog.Log("StoreCenter", "try to edit precenteges");
            if (!bool.TryParse(newValue, out bool newboolValue))
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
                Answer = new StoreAnswer(StoreEnum.Success, "item " + productName + " discount preseneges become true");
            Answer = new StoreAnswer(StoreEnum.Success, "item " + productName + " discount preseneges become false");
            return discount;
        }
        private Discount EditDiscountDiscountAmountPrivateMehtod(Discount discount, string newValue, string productName)
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
            if ((!discount.Percentages) && (newintValue > DataLayerInstance.GetProductByNameFromStore(_storeName, productName).BasePrice))
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
            Answer = new StoreAnswer(StoreEnum.Success, "item " + productName + " discount amount become " + newValue);
            return discount;
        }
        private Discount EditDiscountEndDatePrivateMethod(Discount discount, string newValue, string productName)
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
            if (endDate.Date < discount.startDate.Date)
            {
                MarketLog.Log("StoreCenter", "can't set end time that is sooner then the discount start time");
                throw new StoreException(DiscountStatus.DatesAreWrong, "can't set end time that is sooner then the discount start time");
            }
            discount.EndDate = endDate;
            MarketLog.Log("StoreCenter", " start date changed successfully");
            Answer = new StoreAnswer(StoreEnum.Success, "item " + productName + " discount End Date become " + endDate);
            return discount;
        }
    }
}