using SadnaSrc.Main;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    public class ModuleGlobalHandler : OutsideModuleService
    {
        static ModuleGlobalHandler instance;
        private int StoreIdCounter;
        private int globalProductID;
        private int globalDiscountCode;
        private int globalLotteryID;
        private int globalLotteryTicketID;
      //  internal LinkedList<Store> allStores { get; set; }
        public StoreDL DataLayer { get; }
        public static ModuleGlobalHandler GetInstance()
        {
            if (instance == null)
            {
                instance = new ModuleGlobalHandler();
                return instance;
            }
            return instance;
        }
        private ModuleGlobalHandler()
        {
            DataLayer = new StoreDL();
            StoreIdCounter = DataLayer.FindMaxStoreId();
            globalProductID = DataLayer.FindMaxProductId();
            globalDiscountCode = DataLayer.FindMaxDiscountId();
            globalLotteryID = DataLayer.FindMaxLotteryId();
            globalLotteryTicketID = DataLayer.FindMaxLotteryTicketId();
        }

        public void AddStore(Store temp)
        {
            DataLayer.AddStore(temp);
        }

        public string[] GetStoreInfo(string store)
        {
            return DataLayer.GetStoreInfo(store);
        }

        public string[] GetStoreStockInfo(string store)
        {
            return DataLayer.GetStoreStockInfo(store);
        }

        public StockListItem GetProductFromStore(string store, string productName)
        {
            return DataLayer.GetProductFromStore(store, productName);
        }
        public bool IsProductNameAvailableInStore(string storeName, string productName)
        {
                Product P = DataLayer.getProductByNameFromStore(storeName, productName);
                return (P == null);
        }
        public string PrintEnum(LotteryTicketStatus status)
        {
            switch (status)
            {
                case LotteryTicketStatus.Cancel: return "CANCEL";
                case LotteryTicketStatus.Winning: return "WINNING";
                case LotteryTicketStatus.Waiting: return "WAITING";
                case LotteryTicketStatus.Losing: return "LOSING";
                default: throw new StoreException(MarketError.LogicError, "Enum value not exists"); //TODO :improve this exception
            }

        }
        public string PrintEnum(discountTypeEnum type)
        {
            switch (type)
            {
                case discountTypeEnum.Hidden: return "HIDDEN";
                case discountTypeEnum.Visible: return "VISIBLE";
                default: throw new StoreException(MarketError.LogicError, "Enum value not exists"); //TODO :improve this exception
            }
        }
        public string PrintEnum(PurchaseEnum purchaseEnum)
        {
            switch (purchaseEnum)
            {
                case PurchaseEnum.Immediate: return "Immediate";
                case PurchaseEnum.Lottery: return "Lottery";
                default: throw new StoreException(MarketError.LogicError, "Enum value not exists"); //TODO :improve this exception
            }
        }
        public discountTypeEnum GetdiscountTypeEnumString(string discountType)
        {
            if (discountType == "HIDDEN")
                return discountTypeEnum.Hidden;
            if (discountType == "VISIBLE")
                return discountTypeEnum.Visible;
            throw new StoreException(MarketError.LogicError, "Enum value not exists"); //TODO :improve this exception
        }
        public PurchaseEnum GetPurchaseEnumString(string purchaseType)
        {
            if (purchaseType == "Immediate")
                return PurchaseEnum.Immediate;
            if (purchaseType == "Lottery")
                return PurchaseEnum.Lottery;
            throw new StoreException(MarketError.LogicError, "Enum value not exists"); //TODO :improve this exception
        }

        internal LotteryTicketStatus GetLotteryStatusString(string lotteryStatus)
        {
            if (lotteryStatus == "CANCEL")
                return LotteryTicketStatus.Cancel;
            if (lotteryStatus == "WINNING")
                return LotteryTicketStatus.Winning;
            if (lotteryStatus == "WAITING")
                return LotteryTicketStatus.Waiting;
            if (lotteryStatus == "LOSING")
                return LotteryTicketStatus.Losing;
            throw new StoreException(MarketError.LogicError, "Enum value not exists"); //TODO :improve this exception
        }


        /**
         * next section is ID handlers
         **/
            public string GetProductID()
        {
            int currentMaxProductId = globalProductID;
            globalProductID++;
            return "P" + currentMaxProductId;
        }
        public string GetDiscountCode()
        {
            int currentMaxDiscountCode = globalDiscountCode;
            globalDiscountCode++;
            return "D" + currentMaxDiscountCode;
        }
        public string GetNextStoreId()
        {
            int currentMaxStoreId = StoreIdCounter;
            StoreIdCounter++;
            return "S" + currentMaxStoreId;
        }
        public string GetLottyerID()
        {
            int currentMaxLotteryId = globalLotteryID;
            globalLotteryID++;
            return "L" + currentMaxLotteryId;
        }
        public string GetLotteryTicketID()
        {
            int currentMaxLotteryTicketId = globalLotteryTicketID;
            globalLotteryTicketID++;
            return "T" + currentMaxLotteryTicketId;
        }

        public LinkedList<Store> GetAllStores()
        {
            LinkedList<Store> AllStores = DataLayer.GetAllActiveStores();
            return AllStores;
        }

        public void UpdateQuantityAfterPurchase(string storeName, string productName, int quantity)
        {
            Store store = DataLayer.getStorebyName(storeName);
            if (store ==null) { throw new StoreException(StoreSyncStatus.NoStore, "no such store"); }
            StockListItem product = DataLayer.GetProductFromStore(storeName, productName);
            if (product.Quantity < quantity || quantity <= 0)
                { throw new StoreException(StoreSyncStatus.NoProduct, "product doesn't exist in this quantity"); }
            store.UpdateQuanityAfterPurchase(product.Product, quantity);
        }

        public bool ProductExistsInQuantity(string storeName, string product, int quantity)
        {
            StockListItem sli = DataLayer.GetProductFromStore(storeName, product);
            return sli.Quantity >= quantity;
        }

        public Store GetStoreByID(int ID)
        {
            return GetStoreByID("S" + ID);
        }
        public Store GetStoreByID(string ID)
        {
            return DataLayer.GetStorebyID(ID);
        }
        public LinkedList<Product> GetAllMarketProducts()
        {
            LinkedList<Store> AllStores = DataLayer.GetAllActiveStores();
            LinkedList<Product> result = new LinkedList<Product>();
            foreach (Store store in AllStores)
            {
                result = store.AddAllProductsToExistingList(result);
            }
            return result;
        }

       
    }
}
