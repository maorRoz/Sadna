using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public interface IStoreShoppingService
    {
        MarketAnswer OpenStore(string storeName, string storeAddress);

        MarketAnswer ViewStoreInfo(string store);

        MarketAnswer ViewStoreStock(string store);

        MarketAnswer AddProductToCart(string store, string productName, int quantity);

        MarketAnswer AddLotteryTicket(string store, string productName, double amountToPay);

        MarketAnswer CleanSeesion();
    }

    public enum OpenStoreStatus
    {
        Success,
        AlreadyExist,
        InvalidUser
    }

    public enum ViewStoreStatus
    {
        Success,
        NoStore,
        InvalidUser
    }

    public enum AddProductStatus
    {
        Success,
        NoStore,
        InvalidUser,
        NoProduct
    }

    public enum AddLotteryTicketStatus
    {
        Success,
        NoStore,
        InvalidUser,
        NoTicket,
        TooHighSuggestion,
        TooLowSuggestion
    }
}
