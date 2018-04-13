﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class StoreManagementService : IStoreManagementService
    {

        public Store store;
        ModuleGlobalHandler global;
        private IUserSeller _storeManager;
        public string _storeName;




        //TODO: (maor wrote this) on my opinion, you shouldn't have class who deals with shopping and managing. 
        //TODO: its way too complicated and this class is too big already....
        //TODO: you dont need a class who return only MarketAnswer !!!! this isn't an interface for client. only interface for client need this.

        public StoreManagementService(IUserSeller storeManager, string storeName)
        {
            _storeManager = storeManager;
            _storeName = storeName;
            global = ModuleGlobalHandler.GetInstance();
            store = global.DataLayer.getStorebyName(storeName);
        }

        public MarketAnswer CloseStore()
        {
            try
            {
                global.DataLayer.IsStoreExist(_storeName);
            }
            catch (Exception)
            {
                return new StoreAnswer(StoreEnum.StoreNotExists, "the store doesn't exists");
            }
            try
            {
                _storeManager.CanPromoteStoreOwner(); // can do anything
                return store.CloseStore();
            }
            catch (StoreException)
            {
                MarketLog.Log("StoreCenter", "closing store failed");
                return new StoreAnswer(StoreEnum.CloseStoreFail, "Store is not active");
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "closing store failed");
                return new StoreAnswer(StoreEnum.CloseStoreFail, "you have no premmision to do that");
            }
        }


        private void ValidatePromotionEligible(string actions)
        {
            if (actions.Contains("StoreOwner"))
            {
                _storeManager.CanPromoteStoreOwner();
            }
            else
            {
                _storeManager.CanPromoteStoreAdmin();
            }
        }

        //TODO: continue this, find store and make sure it is active in DB/with Store class entity
        public MarketAnswer PromoteToStoreManager(string someoneToPromoteName, string actions)
        {
            MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " attempting to grant " + someoneToPromoteName +
                                         " manager options in Store" + _storeName + ". Validating store activity and existence..");
            try
            {
                global.DataLayer.IsStoreExist(_storeName);
                ValidatePromotionEligible(actions);
                _storeManager.ValidateNotPromotingHimself(someoneToPromoteName);
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " has been authorized. granting " +
                                             someoneToPromoteName + " manager options in Store" + _storeName + "...");
                _storeManager.Promote(someoneToPromoteName, actions);
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " granted " +
                                             someoneToPromoteName + " manager options in Store" + _storeName + "successfully");
                return new StoreAnswer(PromoteStoreStatus.Success, "promote with manager options has been successful!");

            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " tried to promote others in unavailable Store " + _storeName +
                                             "and has been denied. Error message has been created!");
                return new StoreAnswer(PromoteStoreStatus.InvalidStore, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " has no permission to promote " + someoneToPromoteName +
                                  "with manager options in Store" + _storeName + " and therefore has been denied. Error message has been created!");
                return new StoreAnswer((PromoteStoreStatus)e.Status, e.GetErrorMessage());
            }

        }

        public MarketAnswer GetStoreProducts()
        {
            MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " attempting to view the store stock...");
            try
            {
                global.DataLayer.IsStoreExist(_storeName);
                _storeManager.CanManageProducts();
                List<string> productList = new List<string>();
                foreach (Product product in store.GetAllProducts())
                {
                    productList.Add(product.ToString());
                }

                return new StoreAnswer(ManageStoreStatus.Success, "Stock report has been successfully fetched!",
                    productList.ToArray());
            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " tried to view stock in unavailable Store " + _storeName +
                                             "and has been denied. Error message has been created!");
                return new StoreAnswer(ManageStoreStatus.InvalidStore, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " has no permission to view stock in Store"
                                             + _storeName + " and therefore has been denied. Error message has been created!");
                return new StoreAnswer(ManageStoreStatus.InvalidManager, e.GetErrorMessage());
            }
        }

        //TODO: fix this
        public MarketAnswer AddNewProduct(string _name, int _price, string _description, int quantity)
        {
            MarketLog.Log("StoreCenter", "trying to add product to store");
            MarketLog.Log("StoreCenter", "check if store exists");
            if (!global.DataLayer.IsStoreExist(_storeName)) { return new StoreAnswer(StoreEnum.StoreNotExists, "store not exists"); }
            try
            {
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to add products");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if product name avlaiable in the store" + store.Name);
                if (!global.IsProductNameAvailableInStore(_storeName, _name))
                { throw new StoreException(StoreEnum.ProductNameNotAvlaiableInShop, "Product Name is already Exists In Shop"); }
                MarketLog.Log("StoreCenter", " name is avlaiable");
                MarketLog.Log("StoreCenter", " checking that quanitity is positive");
                if (quantity <= 0) { return new StoreAnswer(StoreEnum.quantityIsNegatie, "negative quantity"); }
                MarketLog.Log("StoreCenter", " quanitity is positive");
                Product product = new Product(global.GetProductID(), _name, _price, _description);
                global.DataLayer.AddStockListItemToDataBase(new StockListItem(quantity, product, null, PurchaseEnum.Immediate, store.SystemId));
                MarketLog.Log("StoreCenter", "product added");
                return new StoreAnswer(StoreEnum.Success, "product added");
            }
            catch (StoreException)
            {
                return new StoreAnswer(StoreEnum.ProductNotFound, "Product Name is already Exists In Shop");
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                return new StoreAnswer(StoreEnum.NoPremmision, "you have no premmision to do that");
            }
        }

        //TODO: fix this
        public MarketAnswer RemoveProduct(string productName)
        {
            MarketLog.Log("StoreCenter", "trying to remove product from store");
            MarketLog.Log("StoreCenter", "check if store exists");
            if (!global.DataLayer.IsStoreExist(_storeName)) { return new StoreAnswer(StoreEnum.StoreNotExists, "store not exists"); }
            try
            {
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to remove products");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if product name exists in the store " + store.Name);
                Product product = global.DataLayer.getProductByNameFromStore(_storeName, productName);
                if (product == null) { MarketLog.Log("StoreCenter", "product not exists"); return new StoreAnswer(StoreEnum.ProductNotFound, "no Such Product"); }
                MarketLog.Log("StoreCenter", "product exists");
                StockListItem stockListItem = global.DataLayer.GetProductFromStore(_storeName, productName);
                if (stockListItem.PurchaseWay == PurchaseEnum.Lottery)
                {
                    LotterySaleManagmentTicket LotteryManagment = global.DataLayer.GetLotteryByProductID(stockListItem.Product.SystemId);
                    LotteryManagment.InformCancel();
                    global.DataLayer.RemoveLottery(LotteryManagment);
                }
                global.DataLayer.RemoveStockListItem(stockListItem);
                return new StoreAnswer(StoreEnum.Success, "product removed");
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                return new StoreAnswer(ViewStoreStatus.InvalidUser, "you have no premmision to do that");
            }
        }

             

        public MarketAnswer EditProduct(string productName, string whatToEdit, string newValue)
        {
            StoreAnswer result = null;
            MarketLog.Log("StoreCenter", "trying to edit product in store");
            MarketLog.Log("StoreCenter", "check if store exists");
            if (!global.DataLayer.IsStoreExist(_storeName)) { return new StoreAnswer(StoreEnum.StoreNotExists, "store not exists"); }
            try
            {
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to edit products");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if product name exists in the store " + store.Name);
                Product product = global.DataLayer.getProductByNameFromStore(_storeName, productName);
                if (product == null) { MarketLog.Log("StoreCenter", "product not exists"); return new StoreAnswer(StoreEnum.ProductNotFound, "no Such Product"); }
                MarketLog.Log("StoreCenter", "product exists");
                if (whatToEdit == "Name"|| whatToEdit == "name")
                {
                    MarketLog.Log("StoreCenter", "edit name");
                    MarketLog.Log("StoreCenter","checking if new new is avaliabe");
                    if(!global.IsProductNameAvailableInStore(_storeName,newValue))
                    {
                        throw new StoreException(StoreEnum.ProductNameNotAvlaiableInShop, "Product Name is already Exists In Shop"); 
                    }
                    result = new StoreAnswer(StoreEnum.Success, "product " + product.SystemId + " name has been updated to " + newValue);
                    product.Name = newValue;
                }
                if (whatToEdit == "BasePrice" || whatToEdit == "basePrice" || whatToEdit == "Baseprice" || whatToEdit == "baseprice")
                {
                    MarketLog.Log("StoreCenter", "edit price");
                    int newBasePrice;
                    if (!int.TryParse(newValue, out newBasePrice))
                    { throw new StoreException(StoreEnum.UpdateProductFail, "value is not leagal"); }
                    if (newBasePrice <= 0) { return new StoreAnswer(StoreEnum.UpdateProductFail, "price can not be negative"); }
                    result = new StoreAnswer(StoreEnum.Success, "product " + product.SystemId + " price has been updated to " + newValue);
                    product.BasePrice = newBasePrice;
                }
                if (whatToEdit == "Description" || whatToEdit == "desccription")
                {
                    MarketLog.Log("StoreCenter", "edit description");
                    result = new StoreAnswer(StoreEnum.Success, "product " + product.SystemId + " Description has been updated to " + newValue);
                    product.Description = newValue;
                }
                if (result == null) { throw new StoreException(StoreEnum.UpdateProductFail, "no leagal attrebute found"); }
                global.DataLayer.EditProductInDatabase(product);
                return result;
            }
            catch (StoreException exe)
            {
                if (exe.Status == (int)StoreEnum.UpdateProductFail)
                {
                    MarketLog.Log("StoreCenter", "no leagal attrebute or founed non-leagal value");
                    return new StoreAnswer(StoreEnum.UpdateProductFail, "no leagal attrebute found");
                }
                MarketLog.Log("StoreCenter", "name exists in shop");
                return new StoreAnswer(StoreEnum.ProductNameNotAvlaiableInShop, "Product Name is already Exists In Shop"); 
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                return new StoreAnswer(ViewStoreStatus.InvalidUser, "you have no premmision to do that");
            }
        }

        //TODO: fix this
        public MarketAnswer ChangeProductPurchaseWayToImmediate(string productName)
        {
            global.DataLayer.IsStoreExist(_storeName);
            _storeManager.CanManageProducts();
            //  store.EditStockListItem(productName, "PurchaseWay", "IMMEDIATE");
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        //TODO: fix this
        public MarketAnswer ChangeProductPurchaseWayToLottery(string productName, DateTime startDate, DateTime endDate)
        {
            global.DataLayer.IsStoreExist(_storeName);
            _storeManager.CanManageProducts();
            // store.EditStockListItem(productName, "PurchaseWay", "LOTTERY");
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        //TODO: fix this
        public MarketAnswer AddDiscountToProduct(string productName, DateTime startDate, DateTime endDate, int discountAmount, string discountType, bool presenteges)
        {
            MarketLog.Log("StoreCenter", "trying to add discount to product in store");
            MarketLog.Log("StoreCenter", "check if store exists");
            if (!global.DataLayer.IsStoreExist(_storeName)) { return new StoreAnswer(StoreEnum.StoreNotExists, "store not exists"); }
            try
            {
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to edit products");
                _storeManager.CanDeclareDiscountPolicy();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if product name exists in the store " + store.Name);
                Product product = global.DataLayer.getProductByNameFromStore(_storeName, productName);
                if (product == null) { MarketLog.Log("StoreCenter", "product not exists");
                    throw new StoreException(StoreEnum.ProductNotFound, "no Such Product"); }
                MarketLog.Log("StoreCenter", "check if dates are OK");
                if ((startDate< DateTime.Now)|| (endDate < DateTime.Now) || !(startDate < endDate))
                {
                    MarketLog.Log("StoreCenter", "something wrong with the dates");
                    throw new StoreException(DiscountStatus.DatesAreWrong, "dates are not leagal"); 
                }
                MarketLog.Log("StoreCenter", "check that discount amount is OK");
                if (presenteges && (discountAmount > 100))
                {
                    MarketLog.Log("StoreCenter", "discount amount is >=100%");
                    throw new StoreException(DiscountStatus.AmountIsHundredAndpresenteges, "DiscountAmount is >= 100%");
                }
                if (!presenteges && (discountAmount > product.BasePrice)) {
                    MarketLog.Log("StoreCenter", "discount amount is >= product price");
                    throw new StoreException(DiscountStatus.DiscountGreaterThenProductPrice, "DiscountAmount is > then product price");
                }
                StockListItem stockListItem = global.DataLayer.GetProductFromStore(_storeName, product.Name);
                MarketLog.Log("StoreCenter", "check that the product don't have another discount");
                if (stockListItem.Discount!=null)
                {
                    MarketLog.Log("StoreCenter", "the product have another discount");
                    throw new StoreException(DiscountStatus.thereIsAlreadyAnotherDiscount, "the product have another discount");
                }
                Discount discount = new Discount(global.GetDiscountCode(), global.GetdiscountTypeEnumString(discountType), startDate,
                    endDate, discountAmount, presenteges);
                stockListItem.Discount = discount;
                global.DataLayer.AddDiscount(discount);
                global.DataLayer.EditStockInDatabase(stockListItem);
                MarketLog.Log("StoreCenter", "discount added successfully");
                return new StoreAnswer(DiscountStatus.Success, "discount added successfully");
            }
            catch (StoreException exe)
            {
                if (exe.Status==(int)DiscountStatus.AmountIsHundredAndpresenteges)
                    return new StoreAnswer(DiscountStatus.AmountIsHundredAndpresenteges, "DiscountAmount is >= 100%");
                if (exe.Status == (int)DiscountStatus.DiscountGreaterThenProductPrice)
                    return new StoreAnswer(DiscountStatus.DiscountGreaterThenProductPrice, "DiscountAmount is > then product price");
                if (exe.Status == (int)DiscountStatus.thereIsAlreadyAnotherDiscount)
                    return new StoreAnswer(DiscountStatus.thereIsAlreadyAnotherDiscount, "the product have another discount");
                if (exe.Status == (int)DiscountStatus.DatesAreWrong)
                    return new StoreAnswer(DiscountStatus.DatesAreWrong, "dates are not leagal");
                //else
                return new StoreAnswer(DiscountStatus.ProductNotFound, "product not found");
            }
            catch (MarketException)
            {
                return new StoreAnswer(ViewStoreStatus.InvalidUser, "you have no premmision to do that");
            }
        }
        /**
         *
         *
        
        Discount discount = new Discount(handler.GetDiscountCode(), handler.GetdiscountTypeEnumString(discountType),
            startDate, endDate,discountAmount, presenteges);
        StockListItem stockListItem = stock.FindstockListItembyProductID(productID);
        if (stockListItem == null) return new StoreAnswer(StoreEnum.ProductNotFound, "product " + productID + " does not exist in Stock");
        stockListItem.Discount = discount;
        handler.DataLayer.AddDiscount(discount);
        handler.DataLayer.EditStockInDatabase(stockListItem);
        return new StoreAnswer(StoreEnum.Success, "Discount added");
*/

        //  return store.AddDiscountToProduct(productName, startDate, endDate, discountAmount, discountType, presenteges);


        //TODO: fix this
        public MarketAnswer EditDiscount(string productID, string whatToEdit, string newValue)
        {
            global.DataLayer.IsStoreExist(_storeName);
            _storeManager.CanDeclareDiscountPolicy();
            //     return store.EditDiscount(productID, whatToEdit, newValue);
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        //TODO: fix this
        public MarketAnswer RemoveDiscountFromProduct(string productID)
        {
            global.DataLayer.IsStoreExist(_storeName);
            _storeManager.CanDeclareDiscountPolicy();
            // store.RemoveDiscountFromProduct(productID);
            return new StoreAnswer(StoreEnum.UpdateStockFail, "you have no premmision to do that");
        }

        public MarketAnswer ViewStoreHistory()
        {
            MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " attempting to view the store purchase history...");
            try
            {
                global.DataLayer.IsStoreExist(_storeName);
                _storeManager.CanViewPurchaseHistory();
                var historyReport = global.DataLayer.GetHistory(store);
                return new StoreAnswer(ViewStorePurchaseHistoryStatus.Success, "View purchase history has been successful!", historyReport);
            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " tried to view purchase history in unavailable Store " + _storeName +
                                             "and has been denied. Error message has been created!");
                return new StoreAnswer(ManageStoreStatus.InvalidStore, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " has no permission to view purchase history in Store"
                                             + _storeName + " and therefore has been denied. Error message has been created!");
                return new StoreAnswer(ManageStoreStatus.InvalidManager, e.GetErrorMessage());
            }
        }

        public void CleanSession()
        {
            if (store != null)
            {
                global.DataLayer.RemoveStore(store);
            }
        }

        public MarketAnswer AddQuanitityToProduct(string productName, int quantity)
        {
            try
            {
                MarketLog.Log("StoreCenter", "checking that store exists");
                if (!global.DataLayer.IsStoreExist(_storeName)) {
                    MarketLog.Log("StoreCenter", "store not exists");
                    return new StoreAnswer(StoreEnum.StoreNotExists, "store not exists"); }
                MarketLog.Log("StoreCenter", "checking that has premmisions");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", "checking that Product Exists");
                if (global.IsProductNameAvailableInStore(_storeName, productName)) {
                    MarketLog.Log("StoreCenter", "Product Not exists");
                    return new StoreAnswer(StoreEnum.ProductNotFound, "product not exists"); }               
                StockListItem stockListItem = global.DataLayer.GetProductFromStore(_storeName,productName);
                MarketLog.Log("StoreCenter", "checking that quantity is positive");
                if (quantity <= 0) {
                    MarketLog.Log("StoreCenter", "quantity is not positive");
                    return new StoreAnswer(StoreEnum.quantityIsNegatie, "quantity " + quantity + " is less then or equal to 0"); }
                stockListItem.Quantity += quantity;
                global.DataLayer.EditStockInDatabase(stockListItem);
                return new StoreAnswer(StoreEnum.Success, "item " + productName + " added by amound of " + quantity);
            }
            catch (MarketException e)
            {
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " has no permission to view purchase history in Store"
                                             + _storeName + " and therefore has been denied. Error message has been created!");
                return new StoreAnswer(ManageStoreStatus.InvalidManager, e.GetErrorMessage());
            }
        }
    }
}
 