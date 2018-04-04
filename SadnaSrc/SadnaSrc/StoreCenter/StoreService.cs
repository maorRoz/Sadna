using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace SadnaSrc.StoreCenter
{
    /**
     * this class handles the stores, Adding Stores, and so on. 
     * it will hold the connection to the DB
     * it will hold the logger
     * this is the gateway from the system to the StoreCenter Packege
     **/
    public class StoreService : IStoreService
    {
        LinkedList<Store> allStores;
        int StoreIdCounter;
        int globalProductID;
        int globalDiscountCode;
        int globalLotteryID;
        public StoreService()
        {
            StoreIdCounter = 0;
            globalDiscountCode = 0;
            globalProductID = 0;
            globalLotteryID = 0;
            allStores = new LinkedList<Store>();
        }

        public MarketAnswer OpenStore(int owner)
        {
            Store temp = new Store(owner, getNextStoreId(), this);
            allStores.AddLast(temp);
            return new StoreAnswer(StoreEnum.Success, "Store " + temp.SystemId + "opend successfully");
        }

        private LinkedList<string> getStoresOfUser(int user)
        {
            LinkedList<Store> result = new LinkedList<Store>();
            foreach (Store store in allStores)
            {
                if (store.IsOwner(user) && store.IsStoreActive())
                {
                    result.AddLast(store);
                }
            }
            if (result.Count != 0)
                return result.ToString();
            return null;
        }

        public string getStoreByID(int ID)
        {
            foreach (Store store in allStores)
            {
                if (store.SystemId == ID)
                {
                    return store;
                }
            }
            return null;
        }

        /**
         * next section is ID handlers
         **/
        internal int getProductID()
        {
            int temp = globalProductID;
            globalProductID++;
            return temp;
        }
        internal int getDiscountCode()
        {
            int temp = globalDiscountCode;
            globalDiscountCode++;
            return temp;
        }
        private int getNextStoreId()
        {
            int temp = StoreIdCounter;
            StoreIdCounter++;
            return temp;
        }
        internal int getLottyerID()
        {
            int temp = globalLotteryID;
            globalLotteryID++;
            return temp;
        }

        public MarketAnswer CloseStore(string storeID, int ownerOrSystemAdmin)
        {
            return store.CloseStore(ownerOrSystemAdmin);
        }
        public static MarketAnswer StaticCloseStore(string store, int ownerOrSystemAdmin)
        {
            return store.CloseStore(ownerOrSystemAdmin);
        }

        public LinkedList<string> getAllMyStores(int owner)
        {
            LinkedList<Store> result = new LinkedList<Store>();
            foreach (Store store in allStores)
            {
                if (store.IsOwner(owner) && store.IsStoreActive())
                {
                    result.AddLast(store);
                }
            }

            return result;
        }

        public LinkedList<string> getAllStores()
        {
            return allStores;
        }

        public LinkedList<string> getAllMarketProducts()
        {
            LinkedList<Product> result = new LinkedList<Product>();
            foreach (Store store in allStores)
            {
                store.addAllProductsToExistingList(result);
            }
            return result;
        }

        public MarketAnswer PromoteToOwner(string store, int CurrentUser, int someoneToPromote)
        {
            return store.PromoteToOwner(CurrentUser, someoneToPromote);
        }

        public MarketAnswer PromoteToManager(string store, int CurrentUser, int someoneToPromote)
        {
            return store.PromoteToManager(CurrentUser, someoneToPromote);
        }

        public LinkedList<string> getAllStoreProducts(string store)
        {
            return store.getAllStoreProducts();
        } 

        public MarketAnswer AddProduct(string store, string _name, int _price, string _description, int quantity)
        {
            return store.AddProduct(_name, _price, _description, quantity);
        }

        public MarketAnswer IncreaseProductQuantity(string store, string product, int quantity)
        {
            return store.IncreaseProductQuantity(product, quantity);
        }

        public MarketAnswer removeProduct(string store, string product)
        {
            return store.removeProduct(product);
        }

        public MarketAnswer editProductPrice(string store, string product, int newprice)
        {
            return store.editProductPrice(product, newprice);
        }

        public MarketAnswer editProductName(string store, string product, string Name)
        {
            return store.editProductName(product, Name);
        }

        public MarketAnswer editProductDescripiton(string store, string product, string Desccription)
        {
            return store.editProductDescripiton(product, Desccription);
        }

        public MarketAnswer ChangeProductPurchaseWayToImmediate(string store, string product)
        {
            return store.ChangeProductPurchaseWayToImmediate(product);
        }

        public MarketAnswer ChangeProductPurchaseWayToLottery(string store, string product, DateTime StartDate, DateTime EndDate)
        {
            return store.ChangeProductPurchaseWayToLottery(product, StartDate, EndDate);
        }

        public MarketAnswer addDiscountToProduct_VISIBLE(string store, string product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            return store.addDiscountToProduct_VISIBLE(product, _startDate, _EndDate, _DiscountAmount);
        }

        public MarketAnswer addDiscountToProduct_HIDDEN(string store, string product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            return store.addDiscountToProduct_HIDDEN(product, _startDate, _EndDate, _DiscountAmount);
        }

        public MarketAnswer addDiscountToProduct_presenteges_VISIBLE(string store, string product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            return store.addDiscountToProduct_presenteges_VISIBLE(product, _startDate, _EndDate, _DiscountAmount);
        }

        public MarketAnswer addDiscountToProduct_presenteges_HIDDEN(string store, string product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            return store.addDiscountToProduct_presenteges_HIDDEN(product, _startDate, _EndDate, _DiscountAmount);
        }

        public MarketAnswer removeDiscount(string store, string product)
        {
            return store.removeDiscountFormProduct(product);
        }

        public MarketAnswer EditDiscountToPrecenteges(string store, string product)
        {
            return store.EditDiscountToPrecenteges(product);
        }

        public MarketAnswer EditDiscountToNonPrecenteges(string store, string product)
        {
            return store.EditDiscountToNonPrecenteges(product);
        }

        public MarketAnswer EditDiscountToHidden(string store, string product)
        {
            return store.EditDiscountToHidden(product);
        }

        public MarketAnswer EditDiscountToVisible(string store, string product)
        {
            return store.EditDiscountToVisible(product);
        }

        public MarketAnswer EditDiscountAmunt(string store, string product, int amount)
        {
            return store.EditDiscountAmount(product, amount);
        }

        public MarketAnswer EditDiscountStartTime(string store, string product, DateTime _startDate)
        {
            return store.EditDiscountStartTime(product, _startDate);
        }

        public MarketAnswer EditDiscountEndTime(string store, string product, DateTime _EndDate)
        {
            return store.EditDiscountEndTime(product, _EndDate);
        }

        public LinkedList<string> ViewPurchaseHistory(string store)
        {
            return store.ViewPurchaseHistory();
        }
    }
}