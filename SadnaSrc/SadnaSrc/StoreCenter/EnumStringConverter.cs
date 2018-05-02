using SadnaSrc.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    static class EnumStringConverter
    {
        public static string PrintEnum(LotteryTicketStatus status)
        {
            switch (status)
            {
                case LotteryTicketStatus.Cancel: return "CANCEL";
                case LotteryTicketStatus.Winning: return "WINNING";
                case LotteryTicketStatus.Waiting: return "WAITING";
                case LotteryTicketStatus.Losing: return "LOSING";
                default: throw new StoreException(MarketError.LogicError, "Enum value not exists");
            }

        }
        public static string PrintEnum(discountTypeEnum type)
        {
            switch (type)
            {
                case discountTypeEnum.Hidden: return "HIDDEN";
                case discountTypeEnum.Visible: return "VISIBLE";
                default: throw new StoreException(MarketError.LogicError, "Enum value not exists");
            }
        }
        public static string PrintEnum(PurchaseEnum purchaseEnum)
        {
            switch (purchaseEnum)
            {
                case PurchaseEnum.Immediate: return "Immediate";
                case PurchaseEnum.Lottery: return "Lottery";
                default: throw new StoreException(StoreEnum.EnumValueNotExists, "Enum value not exists");
            }
        }
        public static discountTypeEnum GetdiscountTypeEnumString(string discountType)
        {
            switch (discountType)
            {
                case "HIDDEN":
                case "hidden":
                case "Hidden":
                    return discountTypeEnum.Hidden;
                case "VISIBLE":
                case "visible":
                case "Visible":
                    return discountTypeEnum.Visible;
            }

            throw new StoreException(StoreEnum.EnumValueNotExists, "Enum value not exists");
        }
        public static PurchaseEnum GetPurchaseEnumString(string purchaseType)
        {
            switch (purchaseType)
            {
                case "Immediate":
                case "immediate":
                case "IMMEDIATE":
                    return PurchaseEnum.Immediate;
                case "Lottery":
                case "lottery":
                case "LOTTERY":
                    return PurchaseEnum.Lottery;
            }

            throw new StoreException(StoreEnum.EnumValueNotExists, "Enum value not exists");
        }

        static public LotteryTicketStatus GetLotteryStatusString(string lotteryStatus)
        {
            switch (lotteryStatus)
            {
                case "CANCEL":
                case "Cancel":
                case "cancel":
                    return LotteryTicketStatus.Cancel;
                case "WINNING":
                case "Winning":
                case "winning":
                    return LotteryTicketStatus.Winning;
                case "WAITING":
                case "Waiting":
                case "waiting":
                    return LotteryTicketStatus.Waiting;
                case "LOSING":
                case "Losing":
                case "losing":
                    return LotteryTicketStatus.Losing;
            }

            throw new StoreException(StoreEnum.EnumValueNotExists, "Enum value not exists");
        }
    }
}
