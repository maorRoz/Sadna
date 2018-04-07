using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace SadnaSrc.StoreCenter
{
    public class StoreService : IStoreService
    {

        UserService user;
        Store store;
        ModuleGlobalHandler global;
        
        /// /////////////////////////////////////////////////////////////////////////////////////////
        //*************************************this function is proxy and will be removed!********///
        public static User ProxyCreateUser(int number)
        {
            return null;
        }
        public static bool ProxyIHavePremmision(User number)
        {
            return true;
        }
        /// /////////////////////////////////////////////////////////////////////////////////////////


        public StoreService(UserService _user, Store _store)
        {
            user = _user;
            store = _store;
            global = ModuleGlobalHandler.GetInstance();
        }

        public MarketAnswer OpenStore(string name, string address)
        {
            Store temp = new Store(global.GetNextStoreId(), name, address);
            global.AddStore(temp);
            return new StoreAnswer(StoreEnum.Success, "Store " + temp.SystemId + "opend successfully");
        }
        
        public MarketAnswer CloseStore()
        {
            if (ProxyIHavePremmision(user.GetUser())){
                return store.CloseStore();
            }
            return new StoreAnswer(StoreEnum.CloseStoreFail, "you have no premmision to do that");
        }
        public static MarketAnswer StaticCloseStore(string storeString, int ownerOrSystemAdmin) //Maor asked my for this one
        {
            if (ProxyIHavePremmision(ProxyCreateUser(ownerOrSystemAdmin))){
                ModuleGlobalHandler global = ModuleGlobalHandler.GetInstance();
                Store other = global.GetStoreByID(storeString);

                //Need Maor function here!
                return other.CloseStore();
            }
            return new StoreAnswer(StoreEnum.CloseStoreFail, "you have no premmision to do that");
        }

        
        

        public MarketAnswer PromoteToOwner(int someoneToPromote)
        {
            if (ProxyIHavePremmision(user.GetUser())){
                // need here to find the fucntion from Maor that add user to be an owner of the store (using 
                return new StoreAnswer(StoreEnum.Success, "user " + someoneToPromote + " has been premoted to be a owner of store " + store.SystemId);
            }            return new StoreAnswer(StoreEnum.AddStoreOwnerFail, "you have no premmision to do that");
        }

        public MarketAnswer PromoteToManager(int someoneToPromote, string actions)
        {
            if (ProxyIHavePremmision(user.GetUser())){
                return new StoreAnswer(StoreEnum.Success, "user " + someoneToPromote + " has been premoted to be a Manager of store " + store.SystemId);
            }
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
            if (ProxyIHavePremmision(user.GetUser()))
            {
                return store.AddProduct(_name, _price, _description, quantity);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer GetProductStockInformation(int productID)
        {
            return store.GetProductStockInformation("P"+ productID);
        }
        
        public MarketAnswer RemoveProduct(string productName)
        {
            if (ProxyIHavePremmision(user.GetUser()))
            {
                return store.RemoveProduct(productName);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer EditProduct(string productName, string whatToEdit, string newValue)
        {
            if (ProxyIHavePremmision(user.GetUser()))
            {
                return store.EditProduct(productName, whatToEdit, newValue);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer ChangeProductPurchaseWayToImmediate(string productName)
        {
            if (ProxyIHavePremmision(user.GetUser()))
            {
                return store.EditStockListItem(productName, "PurchaseWay", "IMMEDIATE");
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer ChangeProductPurchaseWayToLottery(string productName, DateTime startDate, DateTime endDate)
        {
            if (ProxyIHavePremmision(user.GetUser()))
            {
                return store.EditStockListItem(productName, "PurchaseWay", "LOTTERY");
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer AddDiscountToProduct(string productName, DateTime startDate, DateTime endDate, int discountAmount, string discountType, bool presenteges)
        {
            if (ProxyIHavePremmision(user.GetUser()))
            {
                return store.AddDiscountToProduct(productName, startDate, endDate, discountAmount, discountType, presenteges);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer EditDiscount(string productID, string whatToEdit, string newValue)
        {
            if (ProxyIHavePremmision(user.GetUser()))
            {
                return store.EditDiscount(productID, whatToEdit, newValue);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer RemoveDiscountFromProduct(string productID)
        {
            if (ProxyIHavePremmision(user.GetUser()))
            {
                return store.RemoveDiscountFromProduct(productID);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer MakeALotteryPurchase(string productName, int moeny)
        {
            if (ProxyIHavePremmision(user.GetUser()))
            {
                Product product = store.GetProductById(productName);
                if (product==null) { return new StoreAnswer(StoreEnum.ProductNotFound, "no such product"); }
                if (moeny > 0)
                { 
                LotteryTicket loti = store.MakeALotteryPurchase(productName, moeny);
                    if (loti==null) { return new StoreAnswer(StoreEnum.ProductNotFound, "no such product"); }
                    if (!store.CanPurchaseLottery(product,moeny)) { return new StoreAnswer(StoreEnum.PurchesFail, "purching lottery ticket faild"); }
                    user.GetUser().Cart.AddToCart(store.SystemId, loti.ToString(), moeny, "", 1); //ASK MAOR ABOUT IT                    
                return new StoreAnswer(StoreEnum.Success, "lottery ticket sold");
                }
                return new StoreAnswer(StoreEnum.PurchesFail, "cannot pay non-positie amount of moeny");
            }
            return new StoreAnswer(StoreEnum.PurchesFail, "you have no premmision to do that");
        }

        public MarketAnswer MakeAImmediatePurchase(string productName, int discountCode, int quantity)
        {
            if (ProxyIHavePremmision(user.GetUser()))
            {
                Product product = store.MakeAImmediatePurchase(productName, quantity);
                if (product==null) { return new StoreAnswer(StoreEnum.ProductNotFound, "no such product"); }
                double price = store.GetProductPriceWithDiscountbyDouble(productName, discountCode, quantity);
                if (price==-1) { return new StoreAnswer(StoreEnum.ProductNotFound, "no such product"); }
                user.GetUser().Cart.AddToCart(store.SystemId, product.SystemId, price, "", quantity); //ASK MAOR ABOUT IT
                return new StoreAnswer(StoreEnum.Success, "product "+ productName+" sold");
            }
            return new StoreAnswer(StoreEnum.PurchesFail, "you have no premmision to do that");
        }

        public MarketAnswer GetProductPriceWithDiscount(string _product, int _DiscountCode, int _quantity)
        {
            return store.GetProductPriceWithDiscount(_product, _DiscountCode, _quantity);
        }

        public MarketAnswer SetManagersActions(string otherUser, string actions)
        {
            if (ProxyIHavePremmision(user.GetUser())) { 
            string notAllowed = "StoreOwner";
            if (actions.Contains(notAllowed)) { return new StoreAnswer(StoreEnum.SetManagerPermissionsFail, "you tryed to make a manager into a store owner"); }
            throw new NotImplementedException(); //Ask Maor about it
            }
            return new StoreAnswer(StoreEnum.SetManagerPermissionsFail, "you have no premmision to do that");
        }

        public MarketAnswer SetStoreName(string name)
        {
            if (ProxyIHavePremmision(user.GetUser()))
                return store.SetStoreName(name);
            return new StoreAnswer(StoreEnum.EditStoreFail, "you have no premmision to do that");
        }

        public MarketAnswer SetStoreAddress(string address)
        {
            if (ProxyIHavePremmision(user.GetUser()))
                return store.SetStoreAddress(address);
        return new StoreAnswer(StoreEnum.EditStoreFail, "you have no premmision to do that");
    }
    }
}