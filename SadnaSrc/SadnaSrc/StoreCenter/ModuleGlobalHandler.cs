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
        int StoreIdCounter;
        int globalProductID;
        int globalDiscountCode;
        int globalLotteryID;
        int globalLotteryTicketID;
      //  internal LinkedList<Store> allStores { get; set; }
        internal StoreDL dataLayer { get; }
        public static ModuleGlobalHandler getInstance()
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
            dataLayer = new StoreDL();
        }

        internal void AddStore(Store temp)
        {
            dataLayer.addStore(temp);
        }
        internal string PrintEnum(LotteryTicketStatus status)
        {
            if (status == LotteryTicketStatus.CANCEL)
                return "CANCEL";
            if (status == LotteryTicketStatus.WINNING)
                return "WINNING";
            if (status == LotteryTicketStatus.WAITING)
                return "WAITING";
            if (status == LotteryTicketStatus.LOSING)
                return "LOSING";
            return "";
        }
        internal string PrintEnum(discountTypeEnum type)
        {
            if (type == discountTypeEnum.HIDDEN)
                return "HIDDEN";
            if (type == discountTypeEnum.VISIBLE)
                return "VISIBLE";
            return "";
        }
        internal string PrintEnum(PurchaseEnum purchaseEnum)
        {
            if (purchaseEnum == PurchaseEnum.IMMEDIATE) return "IMMEDIATE";
            if (purchaseEnum == PurchaseEnum.LOTTERY) return "LOTTERY";
            throw new StoreException(1, "Enum value not exists");
        }
        internal discountTypeEnum GetdiscountTypeEnumString(String astring)
        {
            if (astring == "HIDDEN")
                return discountTypeEnum.HIDDEN;
            if (astring == "VISIBLE")
                return discountTypeEnum.VISIBLE;
            throw new StoreException(1, "Enum value not exists");
        }
        internal PurchaseEnum GetPurchaseEnumString(String astring)
        {
            if (astring == "IMEMIDIATE")
                return PurchaseEnum.IMMEDIATE;
            if (astring == "LOTTERY")
                return PurchaseEnum.LOTTERY;
            throw new StoreException(1, "Enum not exists");
        }


        /**
         * next section is ID handlers
         **/
        internal string getProductID()
        {
            int temp = globalProductID;
            globalProductID++;
            return "P" + temp;
        }
        internal string getDiscountCode()
        {
            int temp = globalDiscountCode;
            globalDiscountCode++;
            return "D" + temp;
        }
        internal string getNextStoreId()
        {
            int temp = StoreIdCounter;
            StoreIdCounter++;
            return "S" + temp;
        }
        internal string getLottyerID()
        {
            int temp = globalLotteryID;
            globalLotteryID++;
            return "L" + temp;
        }
        internal string getLotteryTicketID()
        {
            int temp = globalLotteryTicketID;
            globalLotteryTicketID++;
            return "T" + temp;
        }

        public LinkedList<Store> getAllUserStores(User user) // this implementation will be change after maor finish his work
        {
            LinkedList<Store> AllStores = dataLayer.getAllActiveStores();
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

        public LinkedList<Store> getAllStores()
        {
            LinkedList<Store> AllStores = dataLayer.getAllActiveStores();
            return AllStores;
        }

        public void UpdateQuantityAfterPurches(string storeID, string productID, int quantity)
        {
            Store store = dataLayer.getStore(storeID);
            if (store ==null) { throw new StoreException(-1, "no such store"); }
            Product product = dataLayer.getProductID(productID);
            if (product==null) { throw new StoreException(-1, "no such product"); }
            store.updateQuanityAfterPurches(product, quantity);
        }
        public Store getStoreByID(int ID)
        {
            return getStoreByID("S" + ID);
        }
        public Store getStoreByID(string ID)
        {
            return dataLayer.getStore(ID);
        }
        
        public LinkedList<Product> getAllMarketProducts()
        {
            LinkedList<Store> AllStores = dataLayer.getAllActiveStores();
            LinkedList<Product> result = new LinkedList<Product>();
            foreach (Store store in AllStores)
            {
                result = store.addAllProductsToExistingList(result);
            }
            return result;
        }

       
    }
}
