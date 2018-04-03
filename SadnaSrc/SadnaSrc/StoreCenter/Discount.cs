using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    public class Discount
    {
        public int discountCode { get; }
        public discountTypeEnum discountType { get; set; }
        public DateTime startDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DiscountAmount { get; set; }
        public bool Presenteges { get; set; }

        public static String PrintEnum(discountTypeEnum type)
        {
            if (type == discountTypeEnum.HIDDEN)
                return "HIDDEN";
            if (type == discountTypeEnum.VISIBLE)
                return "VISIBLE";
            return "";
        }
        //assume that the startDate < EndDate
        public Discount(int _discountCode, discountTypeEnum _discountType, DateTime _EndDate, int _DiscountAmount, bool _presenteges)
        {
            discountCode = _discountCode;
            discountType = _discountType;
            startDate = DateTime.Now.Date;
            EndDate = _EndDate;
            DiscountAmount = _DiscountAmount;
            Presenteges = Presenteges;

        }
        public Discount(int _discountCode, discountTypeEnum _discountType, DateTime _startDate, DateTime _EndDate, int _DiscountAmount, bool _presenteges)
        {
            discountCode = _discountCode;
            discountType = _discountType;
            startDate = _startDate;
            EndDate = _EndDate;
            DiscountAmount = _DiscountAmount;
            Presenteges = Presenteges;

        }
        //assume that the User discountCode is the right one, the check is done by the Store itself
        public double calcDiscount(int basePrice, int code)
        {
            if (discountType==discountTypeEnum.HIDDEN)
            {
                return calculateHiddenDiscount(basePrice, code);
            }
            if (discountType == discountTypeEnum.VISIBLE)
            {
                return calculateVisibleDiscount(basePrice);
            }
            return -1;
        }
        public double calculateHiddenDiscount(int basePrice, int code)
        {
            
            if ((DateTime.Now.Date<EndDate)&&(DateTime.Now.Date>startDate)&&(discountCode==code))
                {
                return calculateVisibleDiscount(basePrice);
                }
            return basePrice;
        }
        public double calculateVisibleDiscount (int basePrice)
        {
            if (Presenteges)
            {
                return basePrice * (1 - DiscountAmount);
            }
            else
            {
                return basePrice - DiscountAmount;
            }
        }
        public String toString() { return "" + DiscountAmount; }
    }
}

