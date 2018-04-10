using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public interface IStoreManagementService
    {

        /**
        * Store Managment, general Actions
        **/
        MarketAnswer PromoteToStoreOwner(string someoneToPromoteName);
        MarketAnswer PromoteToManager(int someoneToPromoteID, string actions);
        MarketAnswer GetProductStockInformation(int ProductID);
        MarketAnswer CloseStore();

        MarketAnswer SetStoreName(string name);
        MarketAnswer SetStoreAddress(string address);

        /**
         * Store Managment, handling products
         **/
        MarketAnswer AddProduct(string _name, int _price, string _description, int quantity);
        MarketAnswer RemoveProduct(string productID);
        MarketAnswer EditProduct(string productID, string whatToEdit, string newValue);
        
        /**
         * Store Managment, handling PurchaseWay
         **/

        MarketAnswer ChangeProductPurchaseWayToImmediate(string productID);
        MarketAnswer ChangeProductPurchaseWayToLottery(string productID, DateTime startDate, DateTime endDate);
        /**
         * Store Managment, Discounts
         **/
        //MarketAnswer EditDiscount();

        MarketAnswer SetManagersActions(string otherUser, string actions);
        MarketAnswer AddDiscountToProduct(string productID, DateTime _startDate, DateTime _endDate, 
            int _discountAmount,string discountType, bool presenteges);
        MarketAnswer EditDiscount(string productID, string whatToEdit, string newValue);
        MarketAnswer RemoveDiscountFromProduct(string productID);

        /**
         * Store Managment, Purchase
         **/
        MarketAnswer MakeALotteryPurchase(string productID, int moeny);
        MarketAnswer MakeAImmediatePurchase(string productID, int discountCode, int quantity);
        MarketAnswer GetProductPriceWithDiscount(string productID, int discountCode, int quantity);
        MarketAnswer GetStoreProducts();
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
        ChangePurchaseTypeFail,
        PurchesFail,
        SetManagerPermissionsFail,
        EditStoreFail
    }

    public enum PurchaseEnum { Immediate, Lottery };
    public enum discountTypeEnum { Hidden, Visible };
    public enum LotteryTicketStatus { Waiting, Winning, Losing, Cancel };
}
