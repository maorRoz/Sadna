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
        
        MarketAnswer OpenStore(string storeName, string storeAddress);

        MarketAnswer ViewStoreInfo(string store);

        MarketAnswer ViewStoreStock(string store);

	    MarketAnswer ViewStoreStockAll(string storename);


		MarketAnswer AddProductToCart(string store, string productName, int quantity);

    }

    public enum OpenStoreStatus
    {
        Success,
        AlreadyExist,
        InvalidUser,
        NoDB = 500

    }

    public enum ViewStoreStatus
    {
        Success,
        NoStore,
        InvalidUser,
        NoDB = 500
    }

    public enum AddProductStatus
    {
        Success,
        NoStore,
        InvalidUser,
        NoProduct,
        NoDB = 500
    }

    public enum AddLotteryTicketStatus
    {
        Success,
        NoStore,
        InvalidUser,
        NoTicket,
        TooHighSuggestion,
        TooLowSuggestion,
        NoDB = 500
    }
   
}
