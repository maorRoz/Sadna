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
        MarketAnswer getProductStockInformation(int ProductID);
        MarketAnswer CloseStore();
        

        /**
         * Store Managment, handling products
         **/
        MarketAnswer AddProduct(string _name, int _price, string _description, int quantity);
        MarketAnswer removeProduct(string productName);
        MarketAnswer editProduct(string productName, string WhatToEdit, string NewValue);
        
        /**
         * Store Managment, handling PurchaseWay
         **/

        MarketAnswer ChangeProductPurchaseWayToImmediate(string productName);
        MarketAnswer ChangeProductPurchaseWayToLottery(string productName, DateTime StartDate, DateTime EndDate);
        /**
         * Store Managment, Discounts
         **/
        //MarketAnswer editDiscount();


        MarketAnswer addDiscountToProduct(string productName, DateTime _startDate, DateTime _EndDate, int _DiscountAmount, string DiscountType, bool presenteges);
        MarketAnswer EditDiscount(string productName, string whatToEdit, string NewValue);
        MarketAnswer removeDiscountFormProduct(string productName);

        /**
         * Store Managment, Purchase
         **/
        MarketAnswer MakeALotteryPurchase(string productName, int moeny);
        MarketAnswer MakeAImmediatePurchase(string productName, int quantity);
        MarketAnswer getProductPriceWithDiscount(string _product, int _DiscountCode, int _quantity);
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
        ChangePurchaseTypeFail
    }

    public enum PurchaseEnum { IMMEDIATE, LOTTERY };
    public enum discountTypeEnum { HIDDEN, VISIBLE };
    public enum LotteryTicketStatus { WAITING, WINNING, LOSING, CANCEL };
}
