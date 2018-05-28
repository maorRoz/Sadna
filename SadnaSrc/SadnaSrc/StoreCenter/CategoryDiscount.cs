using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.StoreCenter
{
    public class CategoryDiscount
    {
        public string SystemId { get; }
        // no type here since it's all visible
        public string StoreName { get; }
        public string CategoryName { get; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DiscountAmount { get; set; }
        private static int _globalCategoryDiscountCode = -1;
        public CategoryDiscount(string storeName, string categoryName ,DateTime startDate, DateTime endDate, int discountAmount)
        {
            SystemId = GetDiscountCode();
            StoreName = storeName;
            CategoryName = categoryName;
            StartDate = startDate;
            EndDate = endDate;
            DiscountAmount = discountAmount;
        }
        public CategoryDiscount(string discountCode, string categoryName, string storeName, DateTime startDate, DateTime endDate, int discountAmount)
        {
            SystemId = discountCode;
            StoreName = storeName;
            CategoryName = categoryName;
            StartDate = startDate;
            EndDate = endDate;
            DiscountAmount = discountAmount;

        }
        public double CalcDiscount(double basePrice)
        {
                return basePrice * (1 - ((double)DiscountAmount / 100));
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.GetType() == GetType() && Equals((CategoryDiscount)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (SystemId != null ? SystemId.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (StoreName != null ? StoreName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (CategoryName != null ? CategoryName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ StartDate.GetHashCode();
                hashCode = (hashCode * 397) ^ EndDate.GetHashCode();
                hashCode = (hashCode * 397) ^ DiscountAmount;
                return hashCode;
            }
        }


        private bool Equals(CategoryDiscount obj)
        {
            return obj.SystemId == SystemId && obj.DiscountAmount == DiscountAmount && CategoryName==obj.CategoryName && StoreName == obj.StoreName;
        }
        public override string ToString()
        {
                return "DiscountAmount: " + DiscountAmount + "%" +
                      " Start Date: " + StartDate.Date.ToString("d") + " End Date: " + EndDate.Date.ToString("d") + " Category is: "+ CategoryName + " Store is: " + StoreName;
        }

        internal bool CheckTime()
        {
            return StartDate.Date <= MarketYard.MarketDate && MarketYard.MarketDate <= EndDate.Date;
        }
        

        public object[] GetDiscountValuesArray()
        {
            return new object[]
            {
                SystemId,
                CategoryName,
                StoreName,
                StartDate,
                EndDate,
                DiscountAmount
            };
        }
        private static string GetDiscountCode()
        {
            if (_globalCategoryDiscountCode == -1)
            {
                _globalCategoryDiscountCode = StockSyncher.GetMaxEntityID(StoreDL.Instance.GetAllCategoryDiscountIDs());
            }
            _globalCategoryDiscountCode++;
            return "d" + _globalCategoryDiscountCode;
        }
    }
}