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

        MarketAnswer AddNewLottery(string _name, double _price, string _description, DateTime startDate,
            DateTime endDate);
        /**
         * Category Managment
         **/
        MarketAnswer AddProductToCategory(string productName, string categoryName);
        MarketAnswer RemoveProductFromCategory(string categoryName, string productName);
        /**
         * Discounts Management
         **/

        MarketAnswer AddDiscountToProduct(string productName, DateTime startDate, DateTime endDate, 
        int discountAmount,string discountType, bool presenteges);
        MarketAnswer EditDiscount(string productName, string whatToEdit, string newValue);
        MarketAnswer RemoveDiscountFromProduct(string productName);
        /**
         * History View
         */
        MarketAnswer AddCategoryDiscount(string categoryName, DateTime startDate, DateTime endDate,
            int discountAmount);
        MarketAnswer EditCategoryDiscount(string categoryName, string whatToEdit, string newValue);
        MarketAnswer RemoveCategoryDiscount(string categoryName);

        MarketAnswer ViewPromotionHistory();

        MarketAnswer ViewStoreHistory();
	    MarketAnswer CloseStore();

		/**
		 * Purchase Policy
		 */
	    MarketAnswer CreatePolicy(string type, string subject, string optSubject, string op, string arg1, string optArg);
	    MarketAnswer SavePolicy();
	    MarketAnswer ViewPolicies();
        MarketAnswer ViewPolicies(string store);

        MarketAnswer ViewPoliciesSessions();
		MarketAnswer RemovePolicy(string type, string subject, string optProd);

    }
	public enum StoreEnum
    {
        Success,
        ProductNotFound,
        UpdateProductFail,
        CloseStoreFail,
        StoreNotExists,
        ProductNameNotAvlaiableInShop,
        NoPermission,
        QuantityIsNegative,
        QuantityIsTooBig,
        EnumValueNotExists,
        DatesAreWrong,
        CategoryNotExistsInStore,
        ProductAlreadyInCategory,
        ProductNotInCategory,
        CategoryNotExistsInSystem,
        CategoryDiscountAlreadyExistsInStore,
        CategoryDiscountNotExistsInStore,
        NoDB = 500,
        BadInput = 600
    }
    public enum DiscountStatus
    {
        Success,
        NoStore,
        DatesAreWrong,
        AmountIsHundredAndpresenteges,
        DiscountGreaterThenProductPrice,
        ThereIsAlreadyAnotherDiscount,
        ProductNotFound,
        DiscountNotFound,
        DiscountAmountIsNegativeOrZero,
        DiscountAmountIsNotNumber,
        PrecentegesIsNotBoolean,
        NoLegalAttrebute,
        NoDB = 500,
        BadInput = 600
    }
    public enum ManageStoreStatus
    {
        Success,
        InvalidStore,
        InvalidManager,
        NoDB = 500,
        BadInput = 600

    }

    public enum ViewStorePurchaseHistoryStatus
    {
        Success,
        InvalidStore,
        InvalidManager,
        NoDB = 500,
        BadInput = 600
    }
    public enum StoreSyncStatus
    {
        NoStore,
        NoProduct,
        NoDB = 500,
        BadInput = 600
    }

	public enum PromoteStoreStatus
	{
		Success,
		InvalidStore,
		PromoteSelf,
		PromotionOutOfReach,
		NoAuthority,
		NoUserFound,
		InvalidPromotion,
	    NoDB = 500,
	    BadInput = 600
    }
    public enum CalculateEnum
    {
        Success,
        StoreNotExists,
        ProductNotFound,
        QuantityIsGreaterThenStack,
        ProductHasNoDiscount,
        DiscountCodeIsWrong,
        DiscountExpired,
        DiscountNotStarted,
        QuanitityIsNonPositive,
        DiscountIsNotHidden,
        NoDB = 500,
        BadInput = 600
    }
    public enum ChangeToLotteryEnum
    {
        Success,
        StoreNotExists,
        ProductNotFound,
        LotteryExists,
        DatesAreWrong,
        NoDB = 500,
        BadInput = 600
    }

    public enum EditStorePolicyStatus
    {
        Success,
        InvalidPolicyData,
        NoAuthority,
        NoDB = 500,
        BadInput = 600

    }

    public enum ViewStorePolicyStatus
    {
        Success,
        NoAuthority,
        NoDB = 500,
        BadInput = 600
    }


    public enum PurchaseEnum { Immediate, Lottery };
    public enum DiscountTypeEnum { Hidden, Visible };
    public enum LotteryTicketStatus { Waiting, Winning, Losing, Cancel };
}
