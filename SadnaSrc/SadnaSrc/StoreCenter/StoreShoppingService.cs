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
                return new StoreAnswer(ViewStoreStatus.Success, "Store info has been successfully granted!", storeInfo);
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
        private string GetProductStockInformation(string ProductID)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            StockListItem stockListItem = handler.DataLayer.GetStockListItembyProductID(ProductID);
            if (stockListItem == null)
            {
                MarketLog.Log("storeCenter", "product not exists");
                throw new StoreException(StoreEnum.ProductNotFound, "product " + ProductID + " does not exist in Stock");
            }
            string discount = "";
            string product = stockListItem.Product.ToString();
            if (stockListItem.Discount != null)
                discount = stockListItem.Discount.ToString() + " , ";
            string purchaseWay = handler.PrintEnum(stockListItem.PurchaseWay);
            string quanitity = stockListItem.Quantity + "";
            string result = product + " , " + discount + purchaseWay + " , " + quanitity;
            return result;
        }
        public MarketAnswer ViewStoreStock(string storename)
        {
            MarketLog.Log("StoreCenter", "checking store stack");
            try
            {
                _shopper.ValidateCanBrowseMarket();
                MarketLog.Log("StoreCenter", "check if store exists");
                if (!storeLogic.DataLayer.IsStoreExistAndActive(storename))
                {
                    MarketLog.Log("StoreCenter", "store do not exists");
                    throw new StoreException(StoreEnum.StoreNotExists, "store not exists or active");
                }
                Store store = storeLogic.DataLayer.getStorebyName(storename);
                LinkedList<string> result = new LinkedList<string>();
                LinkedList<string> IDS = storeLogic.DataLayer.GetAllStoreProductsID(store.SystemId);
                foreach (string item in IDS)
                {
                    result.AddLast(GetProductStockInformation(item));
                }
                string[] resultArray = new string[result.Count];
                result.CopyTo(resultArray, 0);
                return new StoreAnswer(StoreEnum.Success, "", resultArray);
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
            MarketLog.Log("StoreCenter", "trying to add something to the cart");

            try
            {
                MarketLog.Log("StoreCenter", "checking if store exists");
                if (!storeLogic.DataLayer.IsStoreExistAndActive(store))
                { throw new StoreException(StoreEnum.StoreNotExists, "store not exists or active"); }
                MarketLog.Log("StoreCenter", "checking if user has premmisions");
                _shopper.ValidateCanBrowseMarket();
                MarketLog.Log("StoreCenter", "checking if product exists");
                if (storeLogic.IsProductNameAvailableInStore(store, productName)) //aka product is NotFiniteNumberException in store
                {
                    MarketLog.Log("StoreCenter", "Product is not exists in the store");
                    throw new StoreException(StoreEnum.ProductNotFound, "product is not exists");
                }
                StockListItem stockListItem = storeLogic.GetProductFromStore(store, productName);
                MarketLog.Log("StoreCenter", "checking that the required quantity is not too big");
                if (quantity > stockListItem.Quantity)
                {
                    MarketLog.Log("StoreCenter", "required quantity is not too big");
                    throw new StoreException(StoreEnum.QuantityIsTooBig, "required quantity is not too big");
                }
                MarketLog.Log("StoreCenter", "checking that the required quantity is not negative or zero");
                if (quantity <= 0)
                {
                    MarketLog.Log("StoreCenter", "required quantity is negative or zero");
                    throw new StoreException(StoreEnum.quantityIsNegatie, "required quantity is negative");
                }
                if (stockListItem.Discount != null)
                {
                    if (stockListItem.Discount.discountType == discountTypeEnum.Visible)
                        if (stockListItem.Discount.checkTime())
                            stockListItem.Product.BasePrice = stockListItem.Discount.CalcDiscount(stockListItem.Product.BasePrice);
                }
                _shopper.AddToCart(stockListItem.Product, store, quantity);
                MarketLog.Log("StoreCenter", "add product successeded");
                return new StoreAnswer(StoreEnum.Success, quantity + " " + productName + " from " + store + "has been" +
                                                                 " successfully added to the user's cart!");
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