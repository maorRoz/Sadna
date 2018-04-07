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
        public double CalcDiscount(int basePrice, int code)
        {
            if (discountType == discountTypeEnum.Hidden)
            {
                {

                    if ((DateTime.Now.Date < EndDate) && (DateTime.Now.Date > startDate) && (discountCode.Equals(code)))
                    {
                        {
                            if (Percentages)
                            {
                                return basePrice * (1 - DiscountAmount);
                            }
                            else
                            {
                                return basePrice - DiscountAmount;
                            }
                        }
                    }
                    return basePrice;
                }
            }
            if (discountType == discountTypeEnum.Visible)
            {
                if (Percentages)
                {
                    return basePrice * (1 - DiscountAmount);
                }
                else
                {
                    return basePrice - DiscountAmount;
                }
            }
            return -1;
        }
        public override string ToString() {if(Percentages) return "" + DiscountAmount+"%"; return "" + DiscountAmount; }
    }
}

