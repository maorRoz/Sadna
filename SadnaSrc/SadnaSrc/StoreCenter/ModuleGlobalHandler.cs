using SadnaSrc.Main;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;
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
        //    allStores = new LinkedList<Store>();
            StoreIdCounter = 1;
            globalProductID = 1;
            globalDiscountCode = 1;
            globalLotteryID = 1;
            globalLotteryTicketID = 1;
            DataLayer = new StoreDL();
        }

        public void AddStore(Store temp)
        {
            DataLayer.AddStore(temp);
        }
        public string PrintEnum(LotteryTicketStatus status)
        {
            switch (status)
            {
                case LotteryTicketStatus.Cancel: return "CANCEL";
                case LotteryTicketStatus.Winning: return "WINNING";
                case LotteryTicketStatus.Waiting: return "WAITING";
                case LotteryTicketStatus.Losing: return "LOSING";
                default: throw new StoreException(1, "Enum value not exists"); //TODO :improve this exception
            }

        }
        public string PrintEnum(discountTypeEnum type)
        {
            switch (type)
            {
                case discountTypeEnum.Hidden: return "HIDDEN";
                case discountTypeEnum.Visible: return "VISIBLE";
                default: throw new StoreException(1, "Enum value not exists"); //TODO :improve this exception
            }
        }
        public string PrintEnum(PurchaseEnum purchaseEnum)
        {
            switch (purchaseEnum)
            {
                case PurchaseEnum.Immediate: return "IMMEDIATE";
                case PurchaseEnum.Lottery: return "LOTTERY";
                default: throw new StoreException(1, "Enum value not exists"); //TODO :improve this exception
            }
        }
        public discountTypeEnum GetdiscountTypeEnumString(String astring)
        {
            if (astring == "HIDDEN")
                return discountTypeEnum.Hidden;
            if (astring == "VISIBLE")
                return discountTypeEnum.Visible;
            throw new StoreException(1, "Enum value not exists");
        }
        public PurchaseEnum GetPurchaseEnumString(String astring)
        {
            if (astring == "IMEMIDIATE")
                return PurchaseEnum.Immediate;
            if (astring == "LOTTERY")
                return PurchaseEnum.Lottery;
            throw new StoreException(1, "Enum not exists");
        }


        /**
         * next section is ID handlers
         **/
        public string GetProductID()
        {
            int temp = globalProductID;
            globalProductID++;
            return "P" + temp;
        }
        public string GetDiscountCode()
        {
            int temp = globalDiscountCode;
            globalDiscountCode++;
            return "D" + temp;
        }
        public string GetNextStoreId()
        {
            int temp = StoreIdCounter;
            StoreIdCounter++;
            return "S" + temp;
        }
        public string GetLottyerID()
        {
            int temp = globalLotteryID;
            globalLotteryID++;
            return "L" + temp;
        }
        public string GetLotteryTicketID()
        {
            int temp = globalLotteryTicketID;
            globalLotteryTicketID++;
            return "T" + temp;
        }

        public LinkedList<Store> GetAllUserStores(User user) // this implementation will be change after maor finish his work
        {
            LinkedList<Store> AllStores = DataLayer.GetAllActiveStores();
            LinkedList<Store> result = new LinkedList<Store>();
            foreach (Store store in AllStores)
            {
                if (store.IsOwner(user))
                {
                    result.AddLast(store);
                }
            }
            return result;
        }

        public LinkedList<Store> GetAllStores()
        {
            LinkedList<Store> AllStores = DataLayer.GetAllActiveStores();
            return AllStores;
        }

        public void UpdateQuantityAfterPurches(string storeID, string productID, int quantity)
        {
            Store store = DataLayer.GetStore(storeID);
            if (store ==null) { throw new StoreException(-1, "no such store"); }
            Product product = DataLayer.GetProductID(productID);
            if (product==null) { throw new StoreException(-1, "no such product"); }
            store.UpdateQuanityAfterPurches(product, quantity);
        }
        public Store GetStoreByID(int ID)
        {
            return GetStoreByID("S" + ID);
        }
        public Store GetStoreByID(string ID)
        {
            return DataLayer.GetStore(ID);
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
