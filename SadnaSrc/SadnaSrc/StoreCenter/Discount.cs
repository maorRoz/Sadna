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
        public DiscountTypeEnum discountType { get; set; }
        public DateTime startDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DiscountAmount { get; set; }
        public bool Percentages { get; set; }
        private static int globalDiscountCode = -1;

        public Discount(DiscountTypeEnum _discountType, DateTime _startDate, DateTime _EndDate, int _discountAmount, bool _presenteges)
        {
            discountCode = GetDiscountCode();
            discountType = _discountType;
            startDate = _startDate;
            EndDate = _EndDate;
            DiscountAmount = _discountAmount;
            Percentages = _presenteges;

        }
        public Discount(string _discountCode, DiscountTypeEnum _discountType, DateTime _startDate, DateTime _EndDate, int _discountAmount, bool _presenteges)
        {
            discountCode = _discountCode;
            discountType = _discountType;
            startDate = _startDate;
            EndDate = _EndDate;
            DiscountAmount = _discountAmount;
            Percentages = _presenteges;

        }
        //assume that the User discountCode is the right one, the check is done by the Store itself
        public double CalcDiscount(double basePrice)
        {
            if (Percentages)
                return basePrice * (1 - ((double)DiscountAmount / 100));
            return basePrice - DiscountAmount;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.GetType() == GetType() && Equals((Discount)obj);
        }


        private bool Equals(Discount obj)
        {
            return obj.discountCode == discountCode && obj.DiscountAmount == DiscountAmount;
        }
        public override string ToString()
        {
            if (EnumStringConverter.PrintEnum(discountType) == EnumStringConverter.PrintEnum(DiscountTypeEnum.Hidden))
                return "type is: hidden";
            if (Percentages)
            {
                return "DiscountAmount: " + DiscountAmount + "%" +
                      " Start Date: " + startDate.Date.ToString("d") + " End Date: " + EndDate.Date.ToString("d") + " type is: visible";
            }
            return "DiscountAmount: " + DiscountAmount +
             " Start Date: " + startDate.Date.ToString("d") + " End Date: " + EndDate.Date.ToString("d") + " type is: visible";

        }

        internal bool CheckTime()
        {
            return startDate.Date <= MarketYard.MarketDate && MarketYard.MarketDate <= EndDate.Date;
        }

        public override int GetHashCode()
        {
            var hashCode = -536786706;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(discountCode);
            hashCode = hashCode * -1521134295 + discountType.GetHashCode();
            hashCode = hashCode * -1521134295 + startDate.GetHashCode();
            hashCode = hashCode * -1521134295 + EndDate.GetHashCode();
            hashCode = hashCode * -1521134295 + DiscountAmount.GetHashCode();
            hashCode = hashCode * -1521134295 + Percentages.GetHashCode();
            return hashCode;
        }

        public object[] GetDiscountValuesArray()
        {
            return new object[]
            {
                discountCode,
                EnumStringConverter.PrintEnum(discountType),
                startDate,
                EndDate,
                DiscountAmount,
                Percentages.ToString()
            };
        }
        private static string GetDiscountCode()
        {
            if (globalDiscountCode == -1)
            {
                globalDiscountCode = StockSyncher.GetMaxEntityID(StoreDL.Instance.GetAllDiscountIDs());
            }
            globalDiscountCode++;
            return "D" + globalDiscountCode;
        }
        public static void RestartDiscountID()
        {
            globalDiscountCode = -1;
        }
    }
}