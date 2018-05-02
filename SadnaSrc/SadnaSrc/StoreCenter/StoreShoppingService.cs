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
        private readonly StoreSyncerImplementation storeLogic;
        private LinkedList<Store> stores;
        public StoreShoppingService(IUserShopper shopper)
        {
            _shopper = shopper;
            storeLogic = StoreSyncerImplementation.GetInstance();
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
            OpenStoreSlave slave = new OpenStoreSlave(_shopper);
            Store S = slave.OpenStore(storeName, address);
            if (S!=null)
                stores.AddLast(S);
            return slave.answer;
        }
        
        public MarketAnswer ViewStoreInfo(string store)
        {
            ViewStoreInfoSlave slave = new ViewStoreInfoSlave(_shopper);
            slave.ViewStoreInfo(store);
            return slave.answer;
        }
     
        
        public MarketAnswer ViewStoreStock(string storename)
        {
                ViewStoreStockSlave slave = new ViewStoreStockSlave(_shopper);
                slave.ViewStoreStock(storename);
                return slave.answer;
        }

        public MarketAnswer AddProductToCart(string store, string productName, int quantity)
        {
                AddProductToCartSlave slave = new AddProductToCartSlave(_shopper);
                slave.AddProductToCart(store, productName, quantity);
                return slave.answer;
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
 