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
        MarketAnswer AddProduct(string _name, int _price, string _description, int quantity);
        MarketAnswer RemoveProduct(string productName);
        MarketAnswer EditProduct(string productName, string whatToEdit, string newValue);
        MarketAnswer ChangeProductPurchaseWayToImmediate(string productName);
        MarketAnswer ChangeProductPurchaseWayToLottery(string productID, DateTime startDate, DateTime endDate);

        /**
         * Discounts Management
         **/

        MarketAnswer AddDiscountToProduct(string productID, DateTime _startDate, DateTime _endDate, 
            int _discountAmount,string discountType, bool presenteges);
        MarketAnswer EditDiscount(string productID, string whatToEdit, string newValue);
        MarketAnswer RemoveDiscountFromProduct(string productID);

        /**
         * History View
         */

        MarketAnswer ViewStoreHistory();

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
        EditStoreFail,
        StoreNotExists,
        ProductNameNotAvlaiableInShop
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

    public enum PurchaseEnum { Immediate, Lottery };
    public enum discountTypeEnum { Hidden, Visible };
    public enum LotteryTicketStatus { Waiting, Winning, Losing, Cancel };
}
