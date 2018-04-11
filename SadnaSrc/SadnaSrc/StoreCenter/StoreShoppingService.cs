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
            MarketLog.Log("StoreCenter", "trying to add new store");
            try
            {
                _shopper.ValidateRegistered();
                MarketLog.Log("StoreCenter", "premission gained");
                Store newStore = new Store(storeLogic.GetNextStoreId(), storeName, address);
                storeLogic.AddStore(newStore);
                MarketLog.Log("StoreCenter", "store was opened");
                _shopper.AddOwnership(storeName);
                stores.AddLast(newStore);
                MarketLog.Log("StoreCenter", "add myself to the store list");
                return new StoreAnswer(OpenStoreStatus.Success, "Store " + storeName + " has been opened successfully");
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
            MarketLog.Log("StoreCenter", "");
            try
            {
                _shopper.ValidateCanBrowseMarket();
                MarketLog.Log("StoreCenter", "");
                string[] storeInfo = storeLogic.GetStoreInfo(store);
                MarketLog.Log("StoreCenter", "");
                return new StoreAnswer(ViewStoreStatus.Success,"Store info has been successfully granted!", storeInfo);
            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "");
                return new StoreAnswer((ViewStoreStatus)e.Status, "Something is wrong with viewing "+ store + 
                                                                  " info by customers . Error message has been created!");
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                return new StoreAnswer(ViewStoreStatus.InvalidUser,
                    "User validation as valid customer has been failed . only valid users can browse market. Error message has been created!");
            }
        }

        //TODO: doesn't work really, were too complicated for me (maor)...
        public MarketAnswer ViewStoreStock(string store)
        {
            MarketLog.Log("StoreCenter", "");
            try
            {
                _shopper.ValidateCanBrowseMarket();
                MarketLog.Log("StoreCenter", "");
                string[] storeStockInfo = storeLogic.GetStoreStockInfo(store);
                MarketLog.Log("StoreCenter", "");
                return new StoreAnswer(ViewStoreStatus.Success, "Store stock has been successfully granted!", storeStockInfo);
            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "");
                return new StoreAnswer((ViewStoreStatus)e.Status, "Store . " +
                                            "something is wrong with viewing " + store + " stock by customers. Error message has been created!");
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                return new StoreAnswer(ViewStoreStatus.InvalidUser,
                    "User validation as valid customer has been failed . only valid users can browse market. Error message has been created!");
            }
        }

        //TODO: doesn't work really, were too complicated for me(maor)...
        public MarketAnswer AddProductToCart(string store, string productName,int quantity)
        {
            MarketLog.Log("StoreCenter","");
            try
            {
                Product porductToFind =  storeLogic.GetProductFromStore(store,productName,quantity);
                MarketLog.Log("StoreCenter", "");
                _shopper.AddToCart(porductToFind, store,quantity);
                MarketLog.Log("StoreCenter", "");
                return new StoreAnswer(AddProductStatus.Success, quantity +" "+ productName +" from "+store+ "has been" +
                                                                 " successfully added to the user's cart!");
            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "");
                return new StoreAnswer((AddProductStatus)e.Status, "There is no product or store or quantity of that type in the market." +
                                                                  " request has been denied. Error message has been created!");
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                return new StoreAnswer(ViewStoreStatus.InvalidUser,
                    "User validation as valid customer has been failed . only valid users can browse market. Error message has been created!");
            }
        }

        //TODO: fix this
        public MarketAnswer AddLotteryTicket(string store, string productName,double amountToPay)
        {
            MarketLog.Log("StoreCenter", "");
            try
            {
                //PorductToFind =  StoreDL.searchProductInStore(store,productName,"Immediate");
               // MarketLog.Log("StoreCenter", "");
                //  _shopper.AddToCart(productName,store,quantity);
               // MarketLog.Log("StoreCenter", "");
                return new StoreAnswer(AddLotteryTicketStatus.Success, amountToPay + "has been paid to a " + productName + " lottery ticket" +
                                                                       " from " + store + "has been" +" successfully!");
            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "");
                return new StoreAnswer((AddProductStatus)e.Status, "There is no product,ticket,store or quantity of that type in the market." +
                                                                   " request has been denied. Error message has been created!");
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                return new StoreAnswer(ViewStoreStatus.InvalidUser,
                    "User validation as valid customer has been failed . only valid users can browse market. Error message has been created!");
            }
        }

        public void CleanSeesion()
        {
            foreach (Store store in stores)
            {
                storeLogic.DataLayer.RemoveStore(store);
            }
        }
    }
}
