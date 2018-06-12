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

	    MarketAnswer ViewStoreStockAll(string store);

	    MarketAnswer SearchProduct(string value, double minPrice, double maxPrice, string category);

		MarketAnswer AddProductToCart(string store, string productName, int quantity);

	    MarketAnswer GetAllCategoryNames();
    }

    public enum OpenStoreStatus
    {
        Success,
        AlreadyExist,
        InvalidUser,
		InvalidData,
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

	public enum SearchProductStatus
	{
		Success,
		NullValue,
		DidntEnterSystem,
		CategoryNotFound,
		PricesInvalid,
		MistakeTipGiven,
		NoDB = 500
	}

	public enum GetCategoriesStatus
	{
		Success,
		DidntEnterSystem,
		NoDB=500
	}

}
