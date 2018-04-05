using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public interface IStoreService
    {
        /**
         * general actions that refer to all the stores/some of them
         **/
        MarketAnswer OpenStore();
//        LinkedList<string> getAllMyStores();
//        LinkedList<string> getAllStores();
//        LinkedList<string> getAllMarketProducts();
//        string getStoreByID(int ID);
    }
    public enum StoreEnum
    {
        Success,
        UpdateStockFail,
        ProductNotFound,
        DiscountNotFound,
        UpdateDiscountFail,
        UpdateProductFail,
        OpenStoreFail,
        AddStoreOwnerFail,
        AddStoreManagerFail,
        CloseStoreFail,
        ChangePurchaseTypeFail
    }
}
