using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class StoreShoppingService : IStoreShoppingService
    {
        private IUserShopper _shopper;
        private readonly ModuleGlobalHandler storeLogic;
        private LinkedList<Store> stores;
        public StoreShoppingService(IUserShopper shopper)
        {
            _shopper = shopper;
            storeLogic = ModuleGlobalHandler.GetInstance();
            stores = new LinkedList<Store>();
        }
        public void LoginShoper(string userName, string password)
        {
            ((UserShopperHarmony)_shopper).LogInShopper(userName, password);
        }
        public void MakeGuest()
        {
            ((UserShopperHarmony)_shopper).MakeGuest();
        }
        public MarketAnswer OpenStore(string storeName, string address)
        {
            try
            {
                OpenStoreSlave slave = new OpenStoreSlave(_shopper);
                Store S = slave.OpenStore(storeName, address);
                stores.AddLast(S);
                return slave.answer;
            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "error in opening store");
                return new StoreAnswer((OpenStoreStatus)e.Status, "Store " + storeName + " creation has been denied. " +
                                                 "something is wrong with adding a new store of that type. Error message has been created!");
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                return new StoreAnswer(OpenStoreStatus.InvalidUser,
                    "User validation as store owner has been failed. only registered users can open new stores. Error message has been created!");
            }
        }
        
        public MarketAnswer ViewStoreInfo(string store)
        {
            try
            {
                ViewStoreInfoSlave slave = new ViewStoreInfoSlave(_shopper);
                slave.ViewStoreInfo(store);
                return slave.answer;
            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "");
                return new StoreAnswer((ViewStoreStatus)e.Status, "Something is wrong with viewing " + store +
                                                                  " info by customers . Error message has been created!");
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                return new StoreAnswer(ViewStoreStatus.InvalidUser,
                    "User validation as valid customer has been failed . only valid users can browse market. Error message has been created!");
            }


        }
     
        
        public MarketAnswer ViewStoreStock(string storename)
        {
            try
            {
                ViewStoreStockSlave slave = new ViewStoreStockSlave(_shopper);
                slave.ViewStoreStock(storename);
                return slave.answer;
            }
            catch (StoreException e)
            {
                return new StoreAnswer(e);
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                return new StoreAnswer(StoreEnum.NoPremmision,
                    "User validation as valid customer has been failed . only valid users can browse market. Error message has been created!");
            }
        }

        public MarketAnswer AddProductToCart(string store, string productName, int quantity)
        {
            try
            {
                AddProductToCartSlave slave = new AddProductToCartSlave(_shopper);
                slave.AddProductToCart(store, productName, quantity);
                return slave.answer;
            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "adding to cart failed");
                return new StoreAnswer((AddProductStatus)e.Status, "There is no product or store or quantity of that type in the market." +
                                                                  " request has been denied. Error message has been created!");
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                return new StoreAnswer(StoreEnum.NoPremmision,
                    "User validation as valid customer has been failed . only valid users can browse market. Error message has been created!");
            }
        }

        public void CleanSeesion()
        {
            foreach (Store store in stores)
            {
                LinkedList<string> items = storeLogic.DataLayer.GetAllStoreProductsID(store.SystemId);
                foreach (string id in items)
                {
                    StockListItem item = storeLogic.DataLayer.GetStockListItembyProductID(id);
                    storeLogic.DataLayer.RemoveStockListItem(item);
                }
                storeLogic.DataLayer.RemoveStore(store);
            }
        }
    }
}
 