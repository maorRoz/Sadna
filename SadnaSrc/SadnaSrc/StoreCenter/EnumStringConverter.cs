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
        static public string PrintEnum(LotteryTicketStatus status)
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
        static public string PrintEnum(discountTypeEnum type)
        {
            switch (type)
            {
                case discountTypeEnum.Hidden: return "HIDDEN";
                case discountTypeEnum.Visible: return "VISIBLE";
                default: throw new StoreException(MarketError.LogicError, "Enum value not exists");
            }
        }
        static public string PrintEnum(PurchaseEnum purchaseEnum)
        {
            switch (purchaseEnum)
            {
                case PurchaseEnum.Immediate: return "Immediate";
                case PurchaseEnum.Lottery: return "Lottery";
                default: throw new StoreException(StoreEnum.EnumValueNotExists, "Enum value not exists");
            }
        }
        static public discountTypeEnum GetdiscountTypeEnumString(string discountType)
        {
            if ((discountType == "HIDDEN") || (discountType == "hidden") || (discountType == "Hidden"))
                return discountTypeEnum.Hidden;
            if ((discountType == "VISIBLE") || (discountType == "visible") || (discountType == "Visible"))
                return discountTypeEnum.Visible;
            throw new StoreException(StoreEnum.EnumValueNotExists, "Enum value not exists");
        }
        static public PurchaseEnum GetPurchaseEnumString(string purchaseType)
        {
            if ((purchaseType == "Immediate") || (purchaseType == "immediate") || (purchaseType == "IMMEDIATE"))
                return PurchaseEnum.Immediate;
            if ((purchaseType == "Lottery") || (purchaseType == "lottery") || (purchaseType == "LOTTERY"))
                return PurchaseEnum.Lottery;
            throw new StoreException(StoreEnum.EnumValueNotExists, "Enum value not exists");
        }

        static public LotteryTicketStatus GetLotteryStatusString(string lotteryStatus)
        {
            if ((lotteryStatus == "CANCEL") || (lotteryStatus == "Cancel") || (lotteryStatus == "cancel"))
                return LotteryTicketStatus.Cancel;
            if ((lotteryStatus == "WINNING") || (lotteryStatus == "Winning") || (lotteryStatus == "winning"))
                return LotteryTicketStatus.Winning;
            if ((lotteryStatus == "WAITING") || (lotteryStatus == "Waiting") || (lotteryStatus == "waiting"))
                return LotteryTicketStatus.Waiting;
            if ((lotteryStatus == "LOSING") || (lotteryStatus == "Losing") || (lotteryStatus == "losing"))
                return LotteryTicketStatus.Losing;
            throw new StoreException(StoreEnum.EnumValueNotExists, "Enum value not exists");
        }
    }
}
