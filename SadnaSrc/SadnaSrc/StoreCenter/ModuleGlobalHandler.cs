using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.MarketFeed;

namespace SadnaSrc.StoreCenter
{
    public class ModuleGlobalHandler : OutsideModuleService
    {
        static ModuleGlobalHandler instance;
        private int globalProductID;
        private int globalDiscountCode;
        private int globalLotteryID;
        private int globalLotteryTicketID;
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
            DataLayer = StoreDL.Instance;
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
                default: throw new StoreException(MarketError.LogicError, "Enum value not exists"); 
            }

        }
        public string PrintEnum(discountTypeEnum type)
        {
            switch (type)
            {
                case discountTypeEnum.Hidden: return "HIDDEN";
                case discountTypeEnum.Visible: return "VISIBLE";
                default: throw new StoreException(MarketError.LogicError, "Enum value not exists"); 
            }
        }
        public string PrintEnum(PurchaseEnum purchaseEnum)
        {
            switch (purchaseEnum)
            {
                case PurchaseEnum.Immediate: return "Immediate";
                case PurchaseEnum.Lottery: return "Lottery";
                default: throw new StoreException(StoreEnum.EnumValueNotExists, "Enum value not exists");
            }
        }
        public discountTypeEnum GetdiscountTypeEnumString(string discountType)
        {
            if ((discountType == "HIDDEN") || (discountType == "hidden") || (discountType == "Hidden"))
                return discountTypeEnum.Hidden;
            if ((discountType == "VISIBLE") || (discountType == "visible") || (discountType == "Visible"))
                return discountTypeEnum.Visible;
            throw new StoreException(StoreEnum.EnumValueNotExists, "Enum value not exists");
        }
        public PurchaseEnum GetPurchaseEnumString(string purchaseType)
        {
            if ((purchaseType == "Immediate") || (purchaseType == "immediate") || (purchaseType == "IMMEDIATE"))
                return PurchaseEnum.Immediate;
            if ((purchaseType == "Lottery") || (purchaseType == "lottery") || (purchaseType == "LOTTERY"))
                return PurchaseEnum.Lottery;
            throw new StoreException(StoreEnum.EnumValueNotExists, "Enum value not exists");
        }

        internal LotteryTicketStatus GetLotteryStatusString(string lotteryStatus)
        {
            if ((lotteryStatus == "CANCEL") || (lotteryStatus == "Cancel") || (lotteryStatus == "cancel"))
                return LotteryTicketStatus.Cancel;
            if ((lotteryStatus == "WINNING") || (lotteryStatus == "Winning") || (lotteryStatus == "winning"))
                return LotteryTicketStatus.Winning;
            if ((lotteryStatus == "WAITING") || (lotteryStatus == "Waiting") || (lotteryStatus == "waiting"))
                return LotteryTicketStatus.Waiting;
            if ((lotteryStatus == "LOSING") || (lotteryStatus == "Losing") || (lotteryStatus == "losing"))
                return LotteryTicketStatus.Losing;
            throw new StoreException(StoreEnum.EnumValueNotExists, "Enum value not exists");
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
            if (store == null) { throw new StoreException(StoreSyncStatus.NoStore, "no such store"); }
            StockListItem product = DataLayer.GetProductFromStore(storeName, productName);
            if (product.Quantity < quantity || quantity <= 0)
            { throw new StoreException(StoreSyncStatus.NoProduct, "product doesn't exist in this quantity"); }
            product.Quantity -= quantity;
            DataLayer.EditStockInDatabase(product);
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

        public double CalculateItemPriceWithDiscount(string storeName, string productName, string _DiscountCode, int _quantity)
        {
            if (!DataLayer.IsStoreExist(storeName))
                throw new StoreException(CalculateEnum.StoreNotExists, "store not exists");
            if (IsProductNameAvailableInStore(storeName, productName))
                throw new StoreException(CalculateEnum.ProductNotFound, "Product Not Found");
            StockListItem item = DataLayer.GetProductFromStore(storeName, productName);
            if (_quantity > item.Quantity)
                throw new StoreException(CalculateEnum.quantityIsGreaterThenStack, "quantity Is Greater Then Stack");
            if (_quantity <= 0)
                throw new StoreException(CalculateEnum.quanitityIsNonPositive, "quanitity is <=0");
            if (item.Discount == null)
                throw new StoreException(CalculateEnum.ProductHasNoDiscount, "product has no discount");
            if (item.Discount.discountCode != _DiscountCode)
                throw new StoreException(CalculateEnum.DiscountCodeIsWrong, "discount code is wrong");
            if (item.Discount.discountType != discountTypeEnum.Hidden)
                throw new StoreException(CalculateEnum.discountIsNotHidden, "discount Is Not Hiddeng");
            if (MarketYard.MarketDate < item.Discount.startDate.Date)
                throw new StoreException(CalculateEnum.DiscountNotStarted, "Discount Time Not Started Yet");
            if (MarketYard.MarketDate > item.Discount.EndDate.Date)
                throw new StoreException(CalculateEnum.DiscountExpired, "discount expired");
            double ans = item.Discount.CalcDiscount(item.Product.BasePrice);
            ans = ans * _quantity;
            return ans;
        }

        public bool HasActiveLottery(string storeName, string productName, double priceWantToPay)
        {
            LotterySaleManagmentTicket Lotto;
            StockListItem item;
            try
            {
                item = DataLayer.GetProductFromStore(storeName, productName);
            }
            catch (Exception)
            { return false; }

            if (item == null)
            {
                return false;
            }

            if (item.PurchaseWay != PurchaseEnum.Lottery)
            {
                return false;
            }

            try
            {
                Lotto = DataLayer.GetLotteryByProductNameAndStore(storeName, productName);
            }
            catch (Exception)
            { return false; }

            if (Lotto == null)
            {
                return false;
            }

            if (!Lotto.IsActive)
            {
                return false;
            }

            if (priceWantToPay <= 0)
            {
                return false;
            }

            if (!Lotto.CanPurchase(priceWantToPay))
            {
                return false;
            }

            if (!Lotto.checkDatesWhenPurches())
            {
                return false;
            }

            return true;
        }

        public void updateLottery(string storeName, string productName, double moneyPayed, string userName, IOrderSyncher syncher,int cheatCode)
        {
            LotterySaleManagmentTicket Lotto = DataLayer.GetLotteryByProductNameAndStore(storeName, productName);
            if (Lotto.updateLottery(moneyPayed, DataLayer.getUserIDFromUserName(userName)))
            {
                var winnerId = Lotto.getWinnerID(cheatCode);
                syncher.CloseLottery(Lotto.Original.Name, Lotto.storeName, winnerId);
                //  var publisher = Publisher.Instance;
                //   publisher.NotifyLotteryFinish(Lotto.SystemID,productName,storeName);
            }
        }
    }
}