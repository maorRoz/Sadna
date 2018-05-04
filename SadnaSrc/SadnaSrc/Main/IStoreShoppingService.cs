using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public interface IStoreShoppingService
    {
        MarketAnswer GetAllStores(); //Maor asked for me to put it here...
        MarketAnswer OpenStore(string storeName, string storeAddress);

        MarketAnswer ViewStoreInfo(string store);

        MarketAnswer ViewStoreStock(string store);

        MarketAnswer AddProductToCart(string store, string productName, int quantity);

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
