using SadnaSrc.Main;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    public class ModuleGlobalHandler
    {
        static ModuleGlobalHandler instance;
        int StoreIdCounter;
        int globalProductID;
        int globalDiscountCode;
        int globalLotteryID;
        internal LinkedList<Store> allStores { get; set; }
        internal StoreDL dataLayer { get; }
        public static ModuleGlobalHandler getInstance()
        {
            if (instance==null) {
                instance = new ModuleGlobalHandler();
                return instance;
             }
            return instance;
        }
        private ModuleGlobalHandler()
        {
            allStores = new LinkedList<Store>();
            StoreIdCounter = 0;
            globalProductID = 0;
            globalDiscountCode = 0;
            globalLotteryID = 0;
            dataLayer = new StoreDL();
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
            return "PRODUCTNOTFOUND";
        }
        internal discountTypeEnum GetdiscountTypeEnumString(String astring)
        {
            if (astring == "HIDDEN")
                return discountTypeEnum.HIDDEN;
            if (astring == "VISIBLE")
                return discountTypeEnum.VISIBLE;
            throw new StoreException(1, "Enum not exists");
        }
        internal PurchaseEnum GetPurchaseEnumString(String astring)
        {
            if (astring == "IMMIDIATE")
                return PurchaseEnum.IMMEDIATE;
            if (astring == "LOTTERY")
                return PurchaseEnum.LOTTERY;
            if (astring == "PRODUCTNOTFOUND")
                return PurchaseEnum.PRODUCTNOTFOUND;
            throw new StoreException(1, "Enum not exists");
        }


        /**
         * next section is ID handlers
         **/
        internal string getProductID()
        {
            int temp = globalProductID;
            globalProductID++;
            return "P"+temp;
        }
        internal string getDiscountCode()
        {
            int temp = globalDiscountCode;
            globalDiscountCode++;
            return "D"+temp;
        }
        internal string getNextStoreId()
        {
            int temp = StoreIdCounter;
            StoreIdCounter++;
            return "S"+temp;
        }
        internal string getLottyerID()
        {
            int temp = globalLotteryID;
            globalLotteryID++;
            return "L"+temp;
        }

    }
}
