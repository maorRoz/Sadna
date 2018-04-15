﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public interface IStoreManagementService
    {
        void CleanSession(); // for tests only

        /**
         *StoreManagers/StoreOwners Promotion
         */
        MarketAnswer PromoteToStoreManager(string someoneToPromoteName, string actions);

        /**
         * Products Management
         **/
        MarketAnswer AddNewProduct(string _name, double _price, string _description, int quantity);
        MarketAnswer RemoveProduct(string productName);
        MarketAnswer EditProduct(string productName, string whatToEdit, string newValue);
        MarketAnswer AddQuanitityToProduct(string productName, int quantity);
        MarketAnswer ChangeProductPurchaseWayToImmediate(string productName);
        MarketAnswer ChangeProductPurchaseWayToLottery(string productName, DateTime startDate, DateTime endDate);

        /**
         * Discounts Management
         **/

        MarketAnswer AddDiscountToProduct(string productName, DateTime _startDate, DateTime _endDate, 
            int _discountAmount,string discountType, bool presenteges);
        MarketAnswer EditDiscount(string productName, string whatToEdit, string newValue);
        MarketAnswer RemoveDiscountFromProduct(string productName);
        /**
         * History View
         */

        MarketAnswer ViewStoreHistory();
	    MarketAnswer CloseStore();

    }
    public enum StoreEnum
    {
        Success,
        UpdateStockFail,
        ProductNotFound,
        DiscountNotFound,
        UpdateProductFail,
        OpenStoreFail,
        AddStoreOwnerFail,
        AddStoreManagerFail,
        CloseStoreFail,
        ChangePurchaseTypeFail,
        PurchesFail,
        SetManagerPermissionsFail,
        EditStoreFail,
        StoreNotExists,
        ProductNameNotAvlaiableInShop,
        NoPremmision,
        quantityIsNegatie,
        QuantityIsTooBig,
        EnumValueNotExists
    }
    public enum DiscountStatus
    {
        Success,
        NoStore,
        NoProduct,
        DatesAreWrong,
        AmountIsHundredAndpresenteges,
        DiscountGreaterThenProductPrice,
        thereIsAlreadyAnotherDiscount,
        ProductNotFound,
        DiscountNotFound,
        discountAmountIsNegativeOrZero,
        discountAmountIsNotNumber,
        precentegesIsNotBoolean,
        NoLegalAttrebute
    }
    public enum ManageStoreStatus
    {
        Success,
        InvalidStore,
        InvalidManager
        
    }

    public enum ViewStorePurchaseHistoryStatus
    {
        Success,
        InvalidStore,
        InvalidManager
    }
    public enum StoreSyncStatus
    {
        NoStore,
        NoProduct
    }

	public enum PromoteStoreStatus
	{
		Success,
		InvalidStore,
		PromoteSelf,
		PromotionOutOfReach,
		NoAuthority,
		NoUserFound,
		InvalidPromotion
	}
    public enum CalculateEnum
    {
        Success,
        StoreNotExists,
        ProductNotFound,
        quantityIsGreaterThenStack,
        ProductHasNoDiscount,
        DiscountCodeIsWrong,
        DiscountExpired,
        DiscountNotStarted,
        quanitityIsNonPositive,
        discountIsNotHidden
    }
    public enum ChangeToLotteryEnum
    {
        Success,
        StoreNotExists,
        ProductNotFound,
        LotteryExists,
    }


    public enum PurchaseEnum { Immediate, Lottery };
    public enum discountTypeEnum { Hidden, Visible };
    public enum LotteryTicketStatus { Waiting, Winning, Losing, Cancel };
}
