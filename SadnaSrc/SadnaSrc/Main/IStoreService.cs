using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public interface IStoreService
    {
        MarketAnswer OpenStore(string name, string );

        /**
        * Store Managment, general Actions
        **/
        MarketAnswer PromoteToOwner(int someoneToPromoteID);
        MarketAnswer PromoteToManager(int someoneToPromoteID, string actions);
        MarketAnswer getProductStockInformation(int ProductID);
        MarketAnswer CloseStore();

        MarketAnswer setStoreName(string name);
        MarketAnswer setStoreAddress(string address);

        /**
         * Store Managment, handling products
         **/
        MarketAnswer AddProduct(string _name, int _price, string _description, int quantity);
        MarketAnswer removeProduct(string productID);
        MarketAnswer editProduct(string productID, string WhatToEdit, string NewValue);
        
        /**
         * Store Managment, handling PurchaseWay
         **/

        MarketAnswer ChangeProductPurchaseWayToImmediate(string productID);
        MarketAnswer ChangeProductPurchaseWayToLottery(string productID, DateTime StartDate, DateTime EndDate);
        /**
         * Store Managment, Discounts
         **/
        //MarketAnswer editDiscount();

        MarketAnswer setManagersActions(string otherUser, string actions);
        MarketAnswer addDiscountToProduct(string productID, DateTime _startDate, DateTime _EndDate, 
            int _DiscountAmount,string DiscountType, bool presenteges);
        MarketAnswer EditDiscount(string productID, string whatToEdit, string NewValue);
        MarketAnswer removeDiscountFromProduct(string productID);

        /**
         * Store Managment, Purchase
         **/
        MarketAnswer MakeALotteryPurchase(string productID, int moeny);
        MarketAnswer MakeAImmediatePurchase(string productID, int DiscountCode, int quantity);
        MarketAnswer getProductPriceWithDiscount(string productID, int DiscountCode, int quantity);
        MarketAnswer getStoreProducts();
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

    public enum PurchaseEnum { IMMEDIATE, LOTTERY };
    public enum discountTypeEnum { HIDDEN, VISIBLE };
    public enum LotteryTicketStatus { WAITING, WINNING, LOSING, CANCEL };
}
