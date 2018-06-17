using System;
using NUnit.Framework;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class EditDiscountSlave : AbstractStoreCenterSlave
    {
        public MarketAnswer answer;
        private Discount discount;
        private string productName;

        public EditDiscountSlave(string storeName, IUserSeller storeManager, IStoreDL storeDL) : base(storeName, storeManager, storeDL)
        {
        }

        public void EditDiscount(string product,string discountCode, bool isHidden, string startDate, string EndDate, string discountAmount, bool isPercentage)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to edit product in store");
                checkIfStoreExistsAndActive();
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to edit products");
                _storeManager.CanDeclareDiscountPolicy();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if product name exists in the store " + _storeName);
                discount = CheckDiscountExists(product);
                productName = product;
                EditAllDiscountFields(discountCode, isHidden, startDate, EndDate, discountAmount, isPercentage);
                answer = new StoreAnswer(StoreEnum.Success, "Product has been updated!");
                DataLayerInstance.EditDiscountInDatabase(discount);
            }
            catch (StoreException exe)
            {
                answer = new StoreAnswer((StoreEnum)exe.Status, exe.GetErrorMessage());
            }
            catch (DataException e)
            {
                answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "No permission");
                answer = new StoreAnswer(StoreEnum.NoPermission, "You have no permission to do that");
            }
        }

        private void EditAllDiscountFields(string discountCode, bool isHidden, string startDate, string EndDate, string discountAmount, bool isPercentage)
        {

            if (discountCode != null)
            {
                CheckDiscountTypeDetails(discountCode, isHidden);
                if (isHidden)
                {
                    discount.discountCode = discountCode;
                    discount.discountType = DiscountTypeEnum.Hidden;
                }
                else
                {
                    discount.discountCode = "";
                    discount.discountType = DiscountTypeEnum.Visible;
                }
                
            }
            if (startDate != null)
            {
                CheckStartDate(startDate);
                discount.startDate = DateTime.Parse(startDate);

            }

            if (EndDate != null)
            {
                CheckEndDate(EndDate);
                discount.EndDate = DateTime.Parse(EndDate);
            }
            if (discountAmount != null)
            {
                CheckDiscountAmountDetails(discountAmount, isPercentage);
                int discAmountNum = Int32.Parse(discountAmount);
                discount.DiscountAmount = discAmountNum;
                discount.Percentages = isPercentage;
            }
           

        }

        private void CheckDiscountTypeDetails(string discountCode, bool isHidden)
        {
            if (isHidden && discountCode != null) return;
            if (!isHidden && discountCode == null) return;
            throw new StoreException(DiscountStatus.InvalidDiscountType, "Invalid Discount type and coupon code combination.");
        }


        private void CheckStartDate(string startDate)
        {
            DateTime startTime = DateTime.MaxValue;
            if (!DateTime.TryParse(startDate, out startTime))
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
        }

        private void CheckEndDate(string EndDate)
        {
            DateTime EndTime = DateTime.MaxValue;
            if (!DateTime.TryParse(EndDate, out EndTime))
            {
                MarketLog.Log("StoreCenter", "Date format is not legal.");
                throw new StoreException(DiscountStatus.DatesAreWrong, "date format is not legal.");
            }
            if (EndTime.Date < MarketYard.MarketDate)
            {
                MarketLog.Log("StoreCenter", "Can't set end time in the past");
                throw new StoreException(DiscountStatus.DatesAreWrong, "can't set start time in the past.");
            }

            if (EndTime.Date < discount.startDate.Date)
            {
                MarketLog.Log("StoreCenter", "Can't set end time that is sooner then the discount start time.");
                throw new StoreException(DiscountStatus.DatesAreWrong, "Can't set start time that is later then the discount end time.");
            }
        }

        private void CheckDiscountAmountDetails(string discountAmount, bool isPercentage)
        {
            int discAmountNum;
            if (Int32.TryParse(discountAmount, out discAmountNum))
            {
                if (isPercentage && 0 < discAmountNum && discAmountNum < 100) return;
                if (!isPercentage && 0 < discAmountNum && 
                    discAmountNum < DataLayerInstance.GetProductByNameFromStore(_storeName, productName).BasePrice) return;
            }
            throw new StoreException(DiscountStatus.InvalidDiscountAmount, "Invalid Discount Amount.");

        }


        private Discount CheckDiscountExists(string productName)
        {
            StockListItem stockListItem = DataLayerInstance.GetProductFromStore(_storeName, productName);
            if (stockListItem == null)
            {
                MarketLog.Log("StoreCenter", "product does not exists");
                throw new StoreException(DiscountStatus.ProductNotFound, "there is no product");
            }
            MarketLog.Log("StoreCenter", " Product exists");
            MarketLog.Log("StoreCenter", "checking that the product has a discount");
            Discount discount = stockListItem.Discount;
            if (discount == null)
            {
                MarketLog.Log("StoreCenter", "discount does not exists");
                throw new StoreException(DiscountStatus.DiscountNotFound, "there is no discount at this product");
            }
            return discount;
        }
    }
}