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
                result.
            }
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
            throw new NotImplementedException();
        }

        public LinkedList<string> ViewPurchesHistory(Store store)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer AddProduct(Store store, string _name, int _price, string _description, int quantity)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer IncreaseProductQuantity(Store store, Product product, int quantity)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer removeProduct(Store store, Product product)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer editProductPrice(Store store, Product product, int newprice)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer editProductName(Store store, Product product, string Name)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer editProductDescripiton(Store store, Product product, string Desccription)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer ChangeProductPurchesWayToImmidiate(Store store, Product product)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer ChangeProductPurchesWayToLottery(Store store, Product product)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer addDiscountToProduct_VISIBLE(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer addDiscountToProduct_HIDDEN(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer addDiscountToProduct_presenteges_VISIBLE(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer addDiscountToProduct_presenteges_HIDDEN(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer removeDiscount(Store store, Product product)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer MakeDiscountPrecenteges(Store store, Product product)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer MakeDiscountNonPrecenteges(Store store, Product product)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer MakeDiscountHidden(Store store, Product product)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer MakeDiscountVisible(Store store, Product product)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer EditDiscountAmunt(Store store, Product product, int amount)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer EditStartTimeAmunt(Store store, Product product, DateTime _startDate)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer EditEndTimeAmunt(Store store, Product product, DateTime _EndDate)
        {
            throw new NotImplementedException();
        }

        public LotteryTicket MakeALotteryPurches(Store store, Product product, int moeny)
        {
            throw new NotImplementedException();
        }

        public Product MakeAImmidiatePurches(Store store, Product product)
        {
            throw new NotImplementedException();
        }
    }