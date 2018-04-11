using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class StoreService : IStoreManagementService
    {

        public Store store;
        ModuleGlobalHandler global;
        private IUserSeller _storeManager;
        private string _storeName;




        //TODO: (maor wrote this) on my opinion, you shouldn't have class who deals with shopping and managing. 
        //TODO: its way too complicated and this class is too big already....
        //TODO: you dont need a class who return only MarketAnswer !!!! this isn't an interface for client. only interface for client need this.

        public StoreService(IUserSeller storeManager, string storeName)
        {
            _storeManager = storeManager;
            _storeName = storeName;
            store = global.DataLayer.getStorebyName(storeName);
        }

        public MarketAnswer CloseStore()
        {
         //   if (ProxyIHavePremmision(user.GetUser())){ //TODO: fix this
        //        return store.CloseStore();
        //          }
            return new StoreAnswer(StoreEnum.CloseStoreFail, "you have no premmision to do that");
        }



        //TODO: continue this, find store and make sure it is active in DB/with Store class entity
        public MarketAnswer PromoteToStoreOwner(string someoneToPromoteName)
        {
            MarketLog.Log("StoreCenter", "User " + _storeManager.GetID() + " attempting to promote "+ someoneToPromoteName+" into " +
                                         "store owner in Store" + _storeName +". Validating store activity and existence..");
            try
            {
                //find store and make sure it is active in DB/with Store class entity
                _storeManager.CanPromoteStoreOwner();
                _storeManager.ValidateNotPromotingHimself(someoneToPromoteName);
                MarketLog.Log("StoreCenter", "User " + _storeManager.GetID() + " has been authorized. promoting " +
                                             someoneToPromoteName + " to " +
                                             "store owner in Store" + _storeName + "...");
                _storeManager.Promote(someoneToPromoteName, "StoreOwner");
                MarketLog.Log("StoreCenter", "User " + _storeManager.GetID() + "made " +
                                             someoneToPromoteName + " into store owner in Store" + _storeName + "successfully");
                return new StoreAnswer(PromoteStoreStatus.Success,"promote to store owner has been successful!");

            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "User " + _storeManager.GetID() + " tried to promote others in unavailable Store "+_storeName +
                                             "and has been denied. Error message has been created!");
                return new StoreAnswer(PromoteStoreStatus.InvalidStore, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                MarketLog.Log("StoreCenter", "User " + _storeManager.GetID() + " has no permission to promote " + someoneToPromoteName +
                                  "into store owner in Store" + _storeName + " and therefore has been denied. Error message has been created!" );
                return new StoreAnswer((PromoteStoreStatus)e.Status,e.GetErrorMessage());
            }

        }

        public MarketAnswer PromoteToManager(int someoneToPromote, string actions)
        {
            //TODO: fix this
            /*  if (ProxyIHavePremmision(user.GetUser())){
                  return new StoreAnswer(StoreEnum.Success, "user " + someoneToPromote + " has been premoted to be a Manager of store " + store.SystemId);
              }*/
            return new StoreAnswer(StoreEnum.AddStoreManagerFail, "you have no premmision to do that");
        }

        public MarketAnswer GetStoreProducts()
        {
            LinkedList<Product> productList = store.GetAllProducts();
            string result = "";
            foreach (Product product in productList)
            {
                result += product.ToString();
            }
            return new StoreAnswer(StoreEnum.Success, result);
        }

        public MarketAnswer AddProduct(string _name, int _price, string _description, int quantity)
        {
            //TODO: fix this
           /* if (ProxyIHavePremmision(user.GetUser()))
            {
                return store.AddProduct(_name, _price, _description, quantity);
            }*/
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer GetProductStockInformation(int productID)
        {
            return store.GetProductStockInformation("P"+ productID);
        }
        
        public MarketAnswer RemoveProduct(string productName)
        {
            //TODO: fix this
            /*if (ProxyIHavePremmision(user.GetUser()))
            {
                return store.RemoveProduct(productName);
            }*/
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer EditProduct(string productName, string whatToEdit, string newValue)
        {
            //TODO: fix this
            /*if (ProxyIHavePremmision(user.GetUser()))
            {
                return store.EditProduct(productName, whatToEdit, newValue);
            }*/
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer ChangeProductPurchaseWayToImmediate(string productName)
        {
            //TODO: fix this
            /*if (ProxyIHavePremmision(user.GetUser()))
            {
                return store.EditStockListItem(productName, "PurchaseWay", "IMMEDIATE");
            }*/
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer ChangeProductPurchaseWayToLottery(string productName, DateTime startDate, DateTime endDate)
        {
            //TODO: fix this
            /*if (ProxyIHavePremmision(user.GetUser()))
            {
                return store.EditStockListItem(productName, "PurchaseWay", "LOTTERY");
            }*/
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer AddDiscountToProduct(string productName, DateTime startDate, DateTime endDate, int discountAmount, string discountType, bool presenteges)
        {
            //TODO: fix this
            /*if (ProxyIHavePremmision(user.GetUser()))
            {
                return store.AddDiscountToProduct(productName, startDate, endDate, discountAmount, discountType, presenteges);
            }*/
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer EditDiscount(string productID, string whatToEdit, string newValue)
        {
            //TODO: fix this
           /* if (ProxyIHavePremmision(user.GetUser()))
            {
                return store.EditDiscount(productID, whatToEdit, newValue);
            }*/
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer RemoveDiscountFromProduct(string productID)
        {
            //TODO: fix this
            /*if (ProxyIHavePremmision(user.GetUser()))
            {
                return store.RemoveDiscountFromProduct(productID);
            }*/
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer MakeALotteryPurchase(string productName, int moeny)
        {
            //TODO: fix this
            /*if (ProxyIHavePremmision(user.GetUser()))
            {
                Product product = store.GetProductById(productName);
                if (product==null) { return new StoreAnswer(StoreEnum.ProductNotFound, "no such product"); }
                if (moeny > 0)
                { 
                LotteryTicket loti = store.MakeALotteryPurchase(productName, moeny, user.GetUser().SystemID);
                    if (loti==null) { return new StoreAnswer(StoreEnum.ProductNotFound, "no such product"); }
                    if (!store.CanPurchaseLottery(product,moeny)) { return new StoreAnswer(StoreEnum.PurchesFail, "purching lottery ticket faild"); }
                    user.GetUser().Cart.AddToCart(store.SystemId, loti.ToString(), moeny, "", 1); //ASK MAOR ABOUT IT                    
                return new StoreAnswer(StoreEnum.Success, "lottery ticket sold");
                }
                return new StoreAnswer(StoreEnum.PurchesFail, "cannot pay non-positie amount of moeny");
            }*/
            return new StoreAnswer(StoreEnum.PurchesFail, "you have no premmision to do that");
        }

        public MarketAnswer MakeAImmediatePurchase(string productName, int discountCode, int quantity)
        {
            //TODO: fix this
            /*if (ProxyIHavePremmision(user.GetUser()))
            {
                Product product = store.MakeAImmediatePurchase(productName, quantity);
                if (product==null) { return new StoreAnswer(StoreEnum.ProductNotFound, "no such product"); }
                double price = store.GetProductPriceWithDiscountbyDouble(productName, discountCode, quantity);
                if (price==-1) { return new StoreAnswer(StoreEnum.ProductNotFound, "no such product"); }
                user.GetUser().Cart.AddToCart(store.SystemId, product.SystemId, price, "", quantity); //ASK MAOR ABOUT IT
                return new StoreAnswer(StoreEnum.Success, "product "+ productName+" sold");
            }*/
            return new StoreAnswer(StoreEnum.PurchesFail, "you have no premmision to do that");
        }

        public MarketAnswer GetProductPriceWithDiscount(string _product, int _DiscountCode, int _quantity)
        {
            return store.GetProductPriceWithDiscount(_product, _DiscountCode, _quantity);
        }

        public MarketAnswer SetManagersActions(string otherUser, string actions)
        {
            try
            {
                _storeManager.CanPromoteStoreOwner();
                _storeManager.ValidateNotPromotingHimself(otherUser);
                int j = privateSetManagersActions(otherUser, actions);
                if (j == 0)
                    return new StoreAnswer(StoreEnum.Success, "Set Manager Action Succeeded");
                throw new StoreException(StoreEnum.SetManagerPermissionsFail, "set premission failed");
            }
            catch (StoreException)
            {
                return new StoreAnswer(StoreEnum.SetManagerPermissionsFail, "user failed in set premission failed");
            }
            catch (MarketException e)
            {
                MarketLog.Log("StoreCenter", "User " + _storeManager.GetID() + " has no permission to promote " + otherUser +
                                  "into store owner in Store" + _storeName + " and therefore has been denied. Error message has been created!");
                return new StoreAnswer(StoreEnum.SetManagerPermissionsFail, "you have no premmision to do that");
            }
        }
            
        private int privateSetManagersActions(string otherUser, string actions)
        {

            string notAllowed = "StoreOwner";
            if (actions.Contains(notAllowed)) { return -1; }
            try
            {
                _storeManager.Promote(otherUser, actions);
                return 0;
            }
            catch (Exception exe)
            { return -1; }           
        }
    }
}