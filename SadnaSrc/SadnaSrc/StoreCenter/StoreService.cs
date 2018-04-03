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

        public StoreService()
        {
            StoreIdCounter = 0;
            globalDiscountCode = 0;
            globalProductID = 0;
            allStores = new LinkedList<Store>();
        }

        public MarketAnswer OpenStore(User owner)
        {
            Store temp = new Store(owner, getNextStoreId(), this);
            allStores.AddLast(temp);
            return new StoreAnswer(StoreEnum.Success, "Store " + temp.SystemId + "opend successfully");
        }

        private LinkedList<Store> getStoresOfUser(User user)
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
                return result;
            return null;
        }

        public Store getStoreByID(int ID)
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

        public MarketAnswer CloseStore(Store store, User ownerOrSystemAdmin)
        {
            return store.CloseStore(ownerOrSystemAdmin);
        }
        public static MarketAnswer StaticCloseStore(Store store, User ownerOrSystemAdmin)
        {
            return store.CloseStore(ownerOrSystemAdmin);
        }

        public LinkedList<Store> getAllUsersStores(User owner)
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

        public LinkedList<Store> getAllStores(User owner)
        {
            return allStores;
        }

        public LinkedList<Product> getAllMarketProducts()
        {
            LinkedList<Product> result = new LinkedList<Product>();
            foreach (Store store in allStores)
            {
                store.addAllProductsToExistingList(result);
            }
            return result;
        }

        public MarketAnswer PromoteToOwner(Store store, User CurrentUser, User someoneToPromote)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer PromoteToManager(Store store, User CurrentUser, User someoneToPromote)
        {
            throw new NotImplementedException();
        }

        public LinkedList<Product> getAllStoreProducts(Store store)
        {
            return store.getAllStoreProducts();
        } 

        public MarketAnswer AddProduct(Store store, string _name, int _price, string _description, int quantity)
        {
            return store.AddProduct(_name, _price, _description, quantity);
        }

        public MarketAnswer IncreaseProductQuantity(Store store, Product product, int quantity)
        {
            return store.IncreaseProductQuantity(product, quantity);
        }

        public MarketAnswer removeProduct(Store store, Product product)
        {
            return store.removeProduct(product);
        }

        public MarketAnswer editProductPrice(Store store, Product product, int newprice)
        {
            return store.editProductPrice(product, newprice);
        }

        public MarketAnswer editProductName(Store store, Product product, string Name)
        {
            return store.editProductName(product, Name);
        }

        public MarketAnswer editProductDescripiton(Store store, Product product, string Desccription)
        {
            return store.editProductDescripiton(product, Desccription);
        }

        public MarketAnswer ChangeProductPurchesWayToImmidiate(Store store, Product product)
        {
            return store.ChangeProductPurchesWayToImmidiate(product);
        }

        public MarketAnswer ChangeProductPurchesWayToLottery(Store store, Product product)
        {
            return store.ChangeProductPurchesWayToLottery(product);
        }

        public MarketAnswer addDiscountToProduct_VISIBLE(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            return store.addDiscountToProduct_VISIBLE(product, _startDate, _EndDate, _DiscountAmount);
        }

        public MarketAnswer addDiscountToProduct_HIDDEN(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            return store.addDiscountToProduct_HIDDEN(product, _startDate, _EndDate, _DiscountAmount);
        }

        public MarketAnswer addDiscountToProduct_presenteges_VISIBLE(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            return store.addDiscountToProduct_presenteges_VISIBLE(product, _startDate, _EndDate, _DiscountAmount);
        }

        public MarketAnswer addDiscountToProduct_presenteges_HIDDEN(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            return store.addDiscountToProduct_presenteges_HIDDEN(product, _startDate, _EndDate, _DiscountAmount);
        }

        public MarketAnswer removeDiscount(Store store, Product product)
        {
            return store.removeDiscountFormProduct(product);
        }

        public MarketAnswer EditDiscountToPrecenteges(Store store, Product product)
        {
            return store.EditDiscountToPrecenteges(product);
        }

        public MarketAnswer EditDiscountToNonPrecenteges(Store store, Product product)
        {
            return store.EditDiscountToNonPrecenteges(product);
        }

        public MarketAnswer EditDiscountToHidden(Store store, Product product)
        {
            return store.EditDiscountToHidden(product);
        }

        public MarketAnswer EditDiscountToVisible(Store store, Product product)
        {
            return store.EditDiscountToVisible(product);
        }

        public MarketAnswer EditDiscountAmunt(Store store, Product product, int amount)
        {
            return store.EditDiscountAmount(product, amount);
        }

        public MarketAnswer EditDiscountStartTime(Store store, Product product, DateTime _startDate)
        {
            return store.EditDiscountStartTime(product, _startDate);
        }

        public MarketAnswer EditDiscountEndTime(Store store, Product product, DateTime _EndDate)
        {
            return store.EditDiscountEndTime(product, _EndDate);
        }

        public LotteryTicket MakeALotteryPurches(Store store, Product product, int moeny)
        {
            throw new NotImplementedException();
        }

        public Product MakeAImmidiatePurches(Store store, Product product)
        {
            throw new NotImplementedException();
        }
        public LinkedList<string> ViewPurchesHistory(Store store)
        {
            throw new NotImplementedException();
        }
    }
}