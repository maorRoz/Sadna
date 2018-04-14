using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.StoreCenter
{
    public class Discount
    {
        public string discountCode { get; }
        public discountTypeEnum discountType { get; set; }
        public DateTime startDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DiscountAmount { get; set; }
        public bool Percentages { get; set; }

        //assume that the startDate < EndDate

        public Discount(string _discountCode, discountTypeEnum _discountType, DateTime _startDate, DateTime _EndDate, int _discountAmount, bool _presenteges)
        {
            discountCode = _discountCode;
            discountType = _discountType;
            startDate = _startDate;
            EndDate = _EndDate;
            DiscountAmount = _discountAmount;
            Percentages = _presenteges;

        }
        //assume that the User discountCode is the right one, the check is done by the Store itself
        public double CalcDiscount(int basePrice)
        {
            if (Percentages)
                return basePrice * (1 - ((double)DiscountAmount/100));
            return basePrice - DiscountAmount;
        }
                
        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(GetType()))
            {
                return (((Discount)obj).discountCode == discountCode) &&
                    (((Discount)obj).DiscountAmount == DiscountAmount);
            }
            return false;
        }
        public override string ToString()
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            if (handler.PrintEnum(discountType) == handler.PrintEnum(discountTypeEnum.Hidden))
                return "discount code: " + discountCode + " type is: hidden";
            if (Percentages)
            {
                return "discount code: " + discountCode + " DiscountAmount: " + DiscountAmount + "%" +
                      " Start Date: " + startDate + " End Date: " + EndDate;
            }
            return "discount code: " + discountCode + " DiscountAmount: " + DiscountAmount +
             " Start Date: " + startDate + " End Date: " + EndDate;

        }

        internal bool checkTime()
        {
            return ((startDate.Date < 
                ard.MarketDate) && (MarketYard.MarketDate < EndDate.Date));
        }
    }
}
