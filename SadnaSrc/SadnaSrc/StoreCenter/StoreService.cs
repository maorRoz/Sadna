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

        UserService user;
        ModuleGlobalHandler global;
        public StoreService(UserService _user)
        {
            user = _user;
            global = ModuleGlobalHandler.getInstance();
        }

        public MarketAnswer OpenStore()
        {
            Store temp = new Store(user.GetUser(), global.getNextStoreId(), this);
            global.allStores.AddLast(temp);
            return new StoreAnswer(StoreEnum.Success, "Store " + temp.SystemId + "opend successfully");
        }

        private LinkedList<string> getStoresOfUser()
        {
            LinkedList<string> result = new LinkedList<string>();
            foreach (Store store in global.allStores)
            {
                if (store.IsOwner(user.GetUser()) && store.IsStoreActive())
                {
                    result.AddLast(store.ToString());
                }
            }
            if (result.Count != 0)
                return result;
            return null;
        }

        public Store getStoreByID(int ID)
        {
            foreach (Store store in global.allStores)
            {
                if (store.SystemId.Equals("S"+ID))
                {
                    return store;
                }
            }
            return null;
        }
        public Store getStoreByID(string ID)
        {
            foreach (Store store in global.allStores)
            {
                if (store.SystemId.Equals(ID))
                {
                    return store;
                }
            }
            return null;
        }
         public MarketAnswer CloseStore(string storeID, int ownerOrSystemAdmin)
        {
            return store.CloseStore(ownerOrSystemAdmin);
        }
        public static MarketAnswer StaticCloseStore(string store, int ownerOrSystemAdmin)
        {
            return store.CloseStore(ownerOrSystemAdmin);
        }

        public LinkedList<string> getAllMyStores()
        {
            LinkedList<string> result = new LinkedList<string>();
            foreach (Store store in global.allStores)
            {
                if (store.IsOwner(user.GetUser()) && store.IsStoreActive())
                {
                    result.AddLast(store.ToString());
                }
            }

            return result;
        }

        public LinkedList<Store> getAllStores()
        {
            return global.allStores;
        }

        public LinkedList<Product> getAllMarketProducts()
        {
            LinkedList<Product> result = new LinkedList<Product>();
            foreach (Store store in global.allStores)
            {
                store.addAllProductsToExistingList(result);
            }
            return result;
        }

        public MarketAnswer PromoteToOwner(string _store, int someoneToPromote)
        {
            Store store = getStoreByID(_store);
            // need here to find the fucntion from Maor that add user to be an owner of the store (using 
            return store.PromoteToOwner(user.GetUser(), someoneToPromote); //need way to make someoneToPromote to User Type
        }

        public MarketAnswer PromoteToManager(string _store, int someoneToPromote)
        {
            Store store = getStoreByID(_store);
            return store.PromoteToManager(user.GetUser().SystemID, someoneToPromote);
        }

        public LinkedList<string> getAllStoreProducts(Store store)
        {
            LinkedList <Product> presult = store.getAllStoreProducts();
            LinkedList<string> result = new LinkedList<string>();
            foreach (Product p in presult)
            {
                result.AddLast(p.toString());
            }
            return result;
        }

        public MarketAnswer AddProduct(string store, string _name, int _price, string _description, int quantity)
        {
            return store.AddProduct(_name, _price, _description, quantity);
        }

        public MarketAnswer IncreaseProductQuantity(string _store, string product, int quantity)
        {
            Store store = getStoreByID(_store);
            return store.IncreaseProductQuantity(product, quantity);
        }

        public MarketAnswer removeProduct(string _store, string product)
        {
            Store store = getStoreByID(_store);
            return store.removeProduct(product);
        }

        /**public MarketAnswer editProductPrice(string _store, string product, int newprice)
        {
            Store store = getStoreByID(_store);
            return store.editProductPrice(product, newprice);
        }

        public MarketAnswer editProductName(string _store, string product, string Name)
        {
            Store store = getStoreByID(_store);
            return store.editProductName(product, Name);
        }

        public MarketAnswer editProductDescripiton(string _store, string product, string Desccription)
        {
            Store store = getStoreByID(_store);
            return store.editProductDescripiton(product, Desccription);
        }**/

        public MarketAnswer ChangeProductPurchaseWayToImmediate(string _store, string product)
        {
            Store store = getStoreByID(_store);
            return store.ChangeProductPurchaseWayToImmediate(product);
        }

        public MarketAnswer ChangeProductPurchaseWayToLottery(string _store, string product, DateTime StartDate, DateTime EndDate)
        {
            Store store = getStoreByID(_store);
            return store.ChangeProductPurchaseWayToLottery(product, StartDate, EndDate);
        }
        public MarketAnswer EditDiscountOfProduct(string store, string ProductID, string whatToEdit, string NewValue)
        {
            Store S = getStoreByID(store);
            return S.EditDiscount(ProductID, whatToEdit, NewValue);
        }
        public MarketAnswer EditProduct(string store, string ProductID, string whatToEdit, string NewValue)
        {
            Store S = getStoreByID(store);
            return S.EditProduct(ProductID, whatToEdit, NewValue);
        }

        public LinkedList<string> ViewPurchaseHistory(Store store) {
            return store.ViewPurchaseHistory();
        }
    }
}
 
 