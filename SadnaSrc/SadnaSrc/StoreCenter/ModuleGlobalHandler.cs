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
            throw new NotImplementedException();
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
            if (purchaseEnum == PurchaseEnum.IMMEDIATE) return "IMMIDIATE";
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
            if (astring == "IMMIDIATE")
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
            LinkedList<Store> AllStores = StoreDL.getAllActiveStores();
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
            LinkedList<Store> AllStores = StoreDL.getAllActiveStores();
            return AllStores;
        }

        
        public Store getStoreByID(int ID)
        {
            return getStoreByID("S" + ID);
        }
        public Store getStoreByID(string ID)
        {
            LinkedList<Store> AllStores = StoreDL.getAllActiveStores();
            foreach (Store store in AllStores)
            {
                if (store.SystemId.Equals(ID))
                {
                    return store;
                }
            }
            return null;
        }
        
        public LinkedList<Product> getAllMarketProducts()
        {
            LinkedList<Store> AllStores = StoreDL.getAllActiveStores();
            LinkedList<Product> result = new LinkedList<Product>();
            foreach (Store store in AllStores)
            {
                store.addAllProductsToExistingList(result);
            }
            return result;
        }

        internal LinkedList<string> getAllStoreProductsID(string systemId)
        {
            throw new NotImplementedException();
        }
    }
}
