using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public interface IStoreService
    {
        MarketAnswer OpenStore();

        /**
        * Store Managment, general Actions
        **/
        MarketAnswer PromoteToOwner(int someoneToPromoteID);
        MarketAnswer PromoteToManager(int someoneToPromoteID);
        MarketAnswer CloseStore(int ownerOrSystemAdmin);


        /**
         * Store Managment, handling products
         **/
        MarketAnswer AddProduct(string _name, int _price, string _description, int quantity);
        MarketAnswer IncreaseProductQuantity(string productName, int quantity);
        MarketAnswer removeProduct(string productName);
        MarketAnswer editProductPrice(string productName, int newprice);
        MarketAnswer editProductName(string productName, String Name);
        MarketAnswer editProductDescripiton(string productName, String Desccription);

        /**
         * Store Managment, handling PurchaseWay
         **/

        MarketAnswer ChangeProductPurchaseWayToImmediate(string productName);
        MarketAnswer ChangeProductPurchaseWayToLottery(string productName, DateTime StartDate, DateTime EndDate);
        /**
         * Store Managment, Discounts
         **/
        //MarketAnswer editDiscount();


        MarketAnswer addDiscountToProduct_VISIBLE(string productName, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer addDiscountToProduct_HIDDEN(string productName, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer addDiscountToProduct_presenteges_VISIBLE(string productName, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer addDiscountToProduct_presenteges_HIDDEN(string productName, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer EditDiscountToPrecenteges(string productName);
        MarketAnswer EditDiscountToNonPrecenteges(string productName);
        MarketAnswer EditDiscountToHidden(string productName);
        MarketAnswer EditDiscountToVisible(string productName);
        MarketAnswer EditDiscountAmount(string productName, int amount);
        MarketAnswer EditDiscountStartTime(string productName, DateTime _startDate);
        MarketAnswer EditDiscountEndTime(string productName, DateTime _EndDate);
        MarketAnswer removeDiscountFormProduct(string productName);

        /**
         * Store Managment, Purchase
         **/
        MarketAnswer MakeALotteryPurchase(string productName, int moeny);
        MarketAnswer MakeAImmediatePurchase(string productName, int quantity);
        MarketAnswer getProductPriceWithDiscount(string _product, int _DiscountCode, int _quantity);
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

    public enum PurchaseEnum { IMMEDIATE, LOTTERY };
    public enum discountTypeEnum { HIDDEN, VISIBLE };
    public enum LotteryTicketStatus { WAITING, WINNING, LOSING, CANCEL };
}
