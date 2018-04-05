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
        public static User proxyCreateUser(int number)
        {
            return null;
        }
        public static bool proxyIHavePremmision(User number)
        {
            return true;
        }
        /// /////////////////////////////////////////////////////////////////////////////////////////


        public StoreService(UserService _user, Store _store)
        {
            user = _user;
            store = _store;
            global = ModuleGlobalHandler.getInstance();
        }

        public MarketAnswer OpenStore()
        {
            Store temp = new Store(user.GetUser(), global.getNextStoreId(), this);
            global.AddStore(temp);
            return new StoreAnswer(StoreEnum.Success, "Store " + temp.SystemId + "opend successfully");
        }
        
        public MarketAnswer CloseStore()
        {
            if (proxyIHavePremmision(user.GetUser())){
                return store.CloseStore();
            }
            return new StoreAnswer(StoreEnum.AddStoreOwnerFail, "you have no premmision to do that");
        }
        public static MarketAnswer StaticCloseStore(string storeString, int ownerOrSystemAdmin) //Maor asked my for this one
        {
            if (proxyIHavePremmision(proxyCreateUser(ownerOrSystemAdmin))){
                ModuleGlobalHandler global = ModuleGlobalHandler.getInstance();
                Store other = global.getStoreByID(storeString);

                //Need Maor function here!
                return other.CloseStore();
            }
            return new StoreAnswer(StoreEnum.AddStoreOwnerFail, "you have no premmision to do that");
        }

        
        

        public MarketAnswer PromoteToOwner(int someoneToPromote)
        {
            if (proxyIHavePremmision(user.GetUser())){ 
            // need here to find the fucntion from Maor that add user to be an owner of the store (using 
            return store.PromoteToOwner(user.GetUser(), proxyCreateUser(someoneToPromote)); //need way to make someoneToPromote to User Type
            }            return new StoreAnswer(StoreEnum.AddStoreOwnerFail, "you have no premmision to do that");
        }

        public MarketAnswer PromoteToManager(int someoneToPromote)
        {
            if (proxyIHavePremmision(user.GetUser())){
                return store.PromoteToManager(user.GetUser(), proxyCreateUser(someoneToPromote));
            }
            return new StoreAnswer(StoreEnum.AddStoreManagerFail, "you have no premmision to do that");
        }

        public MarketAnswer getStoreProducts()
        {
            LinkedList<Product> Plist = store.getAllProducts();
            string result = "";
            foreach (Product P in Plist)
            {
                result += P.toString();
            }
            return new StoreAnswer(StoreEnum.Success, result);
        }

        public MarketAnswer AddProduct(string _name, int _price, string _description, int quantity)
        {
            if (proxyIHavePremmision(user.GetUser()))
            {
                return store.AddProduct(_name, _price, _description, quantity);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer getProductStockInformation(int ProductID)
        {
            return store.getProductStockInformation(ProductID);
        }
        
        public MarketAnswer removeProduct(string productName)
        {
            if (proxyIHavePremmision(user.GetUser()))
            {
                return store.removeProduct(productName);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer editProduct(string productName, string WhatToEdit, string NewValue)
        {
            if (proxyIHavePremmision(user.GetUser()))
            {
                return store.EditProduct(productName, WhatToEdit, NewValue);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer ChangeProductPurchaseWayToImmediate(string productName)
        {
            if (proxyIHavePremmision(user.GetUser()))
            {
                return store.editStockListItem(productName, "PurchaseWay", "IMMEDIATE");
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer ChangeProductPurchaseWayToLottery(string productName, DateTime StartDate, DateTime EndDate)
        {
            if (proxyIHavePremmision(user.GetUser()))
            {
                return store.editStockListItem(productName, "PurchaseWay", "LOTTERY");
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer addDiscountToProduct(string productName, DateTime _startDate, DateTime _EndDate, int _DiscountAmount, string DiscountType, bool presenteges)
        {
            if (proxyIHavePremmision(user.GetUser()))
            {
                return store.addDiscountToProduct(productName, _startDate, _EndDate, _DiscountAmount, DiscountType, presenteges);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer EditDiscount(string productID, string whatToEdit, string NewValue)
        {
            if (proxyIHavePremmision(user.GetUser()))
            {
                return store.EditDiscount(productID, whatToEdit, NewValue);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer removeDiscountFromProduct(string productID)
        {
            if (proxyIHavePremmision(user.GetUser()))
            {
                return store.removeDiscountFromProduct(productID);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer MakeALotteryPurchase(string productName, int moeny)
        {
            if (proxyIHavePremmision(user.GetUser()))
            {
                Product product = store.getProductById(productName);
                if (product==null) { return new StoreAnswer(StoreEnum.ProductNotFound, "no such product"); }
                if (moeny > 0)
                { 
                LotteryTicket loti = store.MakeALotteryPurchase(productName, moeny);
                    if (loti==null) { return new StoreAnswer(StoreEnum.ProductNotFound, "no such product"); }
                    if (!store.canPurchaseLottery(product,moeny)) { return new StoreAnswer(StoreEnum.PurchesFail, "purching lottery ticket faild"); }
                    user.GetUser().Cart.AddToCart(store.SystemId, loti.toString(), moeny, "", 1); //ASK MAOR ABOUT IT                    
                return new StoreAnswer(StoreEnum.Success, "lottery ticket sold");
                }
                return new StoreAnswer(StoreEnum.PurchesFail, "cannot pay non-positie amount of moeny");
            }
            return new StoreAnswer(StoreEnum.PurchesFail, "you have no premmision to do that");
        }

        public MarketAnswer MakeAImmediatePurchase(string productName, int discountCode, int quantity)
        {
            if (proxyIHavePremmision(user.GetUser()))
            {
                Product product = store.MakeAImmediatePurchase(productName, quantity);
                if (product==null) { return new StoreAnswer(StoreEnum.ProductNotFound, "no such product"); }
                double price = store.getProductPriceWithDiscountbyDouble(productName, discountCode, quantity);
                if (price==-1) { return new StoreAnswer(StoreEnum.ProductNotFound, "no such product"); }
                user.GetUser().Cart.AddToCart(store.SystemId, product.SystemId, price, "", quantity); //ASK MAOR ABOUT IT
                store.updateQuanityAfterPurches(product, quantity);
                return new StoreAnswer(StoreEnum.Success, "product "+ productName+" sold");
            }
            return new StoreAnswer(StoreEnum.PurchesFail, "you have no premmision to do that");
        }

        public MarketAnswer getProductPriceWithDiscount(string _product, int _DiscountCode, int _quantity)
        {
            return store.getProductPriceWithDiscount(_product, _DiscountCode, _quantity);
        }

        public MarketAnswer setManagersActions(string otherUser, string actions)
        {
            throw new NotImplementedException(); //Ask Maor about it
        }
    }
}