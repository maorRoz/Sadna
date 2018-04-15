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
    public class StoreManagementService : IStoreManagementService
    {

        public Store store;
        ModuleGlobalHandler global;
        private IUserSeller _storeManager;
        public string _storeName;

        private LinkedList<StockListItem> stockListItemToRemove;
        private LinkedList<Discount> discountsToRemvoe;

        public StoreManagementService(IUserSeller storeManager, string storeName)
        {
            _storeManager = storeManager;
            _storeName = storeName;
            global = ModuleGlobalHandler.GetInstance();
            store = global.DataLayer.getStorebyName(storeName);
            stockListItemToRemove = new LinkedList<StockListItem>();
            discountsToRemvoe = new LinkedList<Discount>();
        }

        public MarketAnswer CloseStore()
        {
            try
            {
                if (!global.DataLayer.IsStoreExist(_storeName)) { return new StoreAnswer(StoreEnum.StoreNotExists, "store not exists"); }
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

        public MarketAnswer PromoteToStoreManager(string someoneToPromoteName, string actions)
        {
            MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " attempting to grant " + someoneToPromoteName +
                                         " manager options in Store" + _storeName + ". Validating store activity and existence..");
            try
            {
                global.DataLayer.ValidateStoreExists(_storeName);
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

      /*  public MarketAnswer GetStoreProducts()
        {
            MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " attempting to view the store stock...");
            try
            {
                if (!global.DataLayer.IsStoreExist(_storeName)) { return new StoreAnswer(StoreEnum.StoreNotExists, "store not exists"); }
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
        }*/

        
        public MarketAnswer AddNewProduct(string _name, double _price, string _description, int quantity)
        {
            MarketLog.Log("StoreCenter", "trying to add product to store");
            MarketLog.Log("StoreCenter", "check if store exists");
            if (!global.DataLayer.IsStoreExistAndActive(_storeName)) { return new StoreAnswer(StoreEnum.StoreNotExists, "store not exists or active"); }
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
                StockListItem stockListItem = new StockListItem(quantity, product, null, PurchaseEnum.Immediate, store.SystemId);
                global.DataLayer.AddStockListItemToDataBase(stockListItem);
                stockListItemToRemove.AddLast(stockListItem);
                MarketLog.Log("StoreCenter", "product added");
                return new StoreAnswer(StoreEnum.Success, "product added");
            }
			catch (StoreException)
            {
                return new StoreAnswer(StoreEnum.ProductNameNotAvlaiableInShop, "Product Name is already Exists In Shop");
            }
			catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                return new StoreAnswer(StoreEnum.NoPremmision, "you have no premmision to do that");
            }
        }

        public MarketAnswer RemoveProduct(string productName)
        {
            MarketLog.Log("StoreCenter", "trying to remove product from store");
            MarketLog.Log("StoreCenter", "check if store exists");
            if (!global.DataLayer.IsStoreExistAndActive(_storeName)) { return new StoreAnswer(StoreEnum.StoreNotExists, "store not exists or active"); }
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
            catch (MarketException e)
            {
                MarketLog.Log("StoreCenter", "no premission");
                return new StoreAnswer(StoreEnum.NoPremmision, "you have no premmision to do that");
            }
        }



        public MarketAnswer EditProduct(string productName, string whatToEdit, string newValue)
        {

            StoreAnswer result = null;
            MarketLog.Log("StoreCenter", "trying to edit product in store");
            MarketLog.Log("StoreCenter", "check if store exists");
            if (!global.DataLayer.IsStoreExistAndActive(_storeName))
            {
                return new StoreAnswer(StoreEnum.StoreNotExists, "store not exists");
            }
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
                if (whatToEdit == "Name" || whatToEdit == "name")
                {
                    MarketLog.Log("StoreCenter", "edit name");
                    MarketLog.Log("StoreCenter", "checking if new new is avaliabe");
                    if (!global.IsProductNameAvailableInStore(_storeName, newValue))
                    {
                        MarketLog.Log("StoreCenter", "name exists in shop");
                        throw new StoreException(StoreEnum.ProductNameNotAvlaiableInShop, "Product Name is already Exists In Shop");
                    }
                    result = new StoreAnswer(StoreEnum.Success, "product " + product.SystemId + " name has been updated to " + newValue);
                    product.Name = newValue;
                }
                if (whatToEdit == "BasePrice" || whatToEdit == "basePrice" || whatToEdit == "Baseprice" || whatToEdit == "baseprice")
                {
                    MarketLog.Log("StoreCenter", "edit price");
                    double newBasePrice;
                    if (!double.TryParse(newValue, out newBasePrice))
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
                if (result == null)
                {
                    MarketLog.Log("StoreCenter", "no leagal attrebute or founed non-leagal value");
                    throw new StoreException(StoreEnum.UpdateProductFail, "no leagal attrebute found");
                }
                global.DataLayer.EditProductInDatabase(product);
                return result;
            }
            catch (StoreException exe)
            {
                return new StoreAnswer(exe);
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                return new StoreAnswer(StoreEnum.NoPremmision, "you have no premmision to do that");
            }
        }
        public void clearSession()
        {
            foreach (Discount discount in discountsToRemvoe)
            {
                global.DataLayer.RemoveDiscount(discount);
            }
            foreach (StockListItem stockListItem in stockListItemToRemove)
            {
                global.DataLayer.RemoveStockListItem(stockListItem);
            }
        }
        public MarketAnswer ChangeProductPurchaseWayToImmediate(string productName)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to edit discount from product in store");
                checkIfStoreExists();
                MarketLog.Log("StoreCenter", " check if has premmision to edit product purches type");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", "check if product exists");
                checkIfProductExists(productName);
                MarketLog.Log("StoreCenter", "product exists");
                StockListItem stockList = global.DataLayer.GetProductFromStore(_storeName, productName);
                if (stockList.PurchaseWay == PurchaseEnum.Lottery)
                {
                    LotterySaleManagmentTicket lottery = global.DataLayer.GetLotteryByProductID(stockList.Product.SystemId);
                    lottery.InformCancel();
                    global.DataLayer.EditLotteryInDatabase(lottery);

                }
                stockList.PurchaseWay = PurchaseEnum.Immediate;
                global.DataLayer.EditStockInDatabase(stockList);
                return new StoreAnswer(StoreEnum.Success, "purches way changed");
            }
            catch (StoreException exe)
            {
                return new StoreAnswer(exe);
            }
        }

        //TODO: fix this
        public MarketAnswer ChangeProductPurchaseWayToLottery(string productName, DateTime startDate, DateTime endDate)
        {
            try
            {
                MarketLog.Log("StoreCenter", "check if store exists");
                checkIfStoreExists();
                MarketLog.Log("StoreCenter", " check if has premmision to edit products");
                _storeManager.CanDeclareDiscountPolicy();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", "check if product exists");
                checkIfProductExists(productName);
                MarketLog.Log("StoreCenter", "product exists");
                StockListItem stockListItem = global.DataLayer.GetProductFromStore(_storeName, productName);
                if (stockListItem.PurchaseWay == PurchaseEnum.Lottery)
                {
                    MarketLog.Log("StoreCenter", " product has a lottery");
                    throw new StoreException(ChangeToLotteryEnum.LotteryExists, "product has a lottery");
                }
                stockListItem.PurchaseWay = PurchaseEnum.Lottery;
                global.DataLayer.EditStockInDatabase(stockListItem);
                LotterySaleManagmentTicket lotterySaleManagmentTicket = new LotterySaleManagmentTicket(global.GetLottyerID(),
                    _storeName, stockListItem.Product, startDate, endDate);
                global.DataLayer.AddLottery(lotterySaleManagmentTicket);

                return new StoreAnswer(ChangeToLotteryEnum.Success, "type changed");
            }
            catch (StoreException exe)
            {
                return new StoreAnswer(exe);
            }
            catch (MarketException exe)
            {
                return new StoreAnswer(StoreEnum.NoPremmision, "you have no premmision to do that");
            }

            }


        public MarketAnswer AddDiscountToProduct(string productName, DateTime startDate, DateTime endDate, int discountAmount, string discountType, bool presenteges)
        {
            MarketLog.Log("StoreCenter", "trying to add discount to product in store");
            MarketLog.Log("StoreCenter", "check if store exists");
            if (!global.DataLayer.IsStoreExistAndActive(_storeName)) { return new StoreAnswer(DiscountStatus.NoStore, "store not exists"); }
            try
            {
                MarketLog.Log("StoreCenter", " store exists");
                MarketLog.Log("StoreCenter", " check if has premmision to edit products");
                _storeManager.CanDeclareDiscountPolicy();
                MarketLog.Log("StoreCenter", " has premmission");
                MarketLog.Log("StoreCenter", " check if product name exists in the store " + store.Name);
                Product product = global.DataLayer.getProductByNameFromStore(_storeName, productName);
                if (product == null)
                {
                    MarketLog.Log("StoreCenter", "product not exists");
                    throw new StoreException(DiscountStatus.ProductNotFound, "no Such Product");
                }
                MarketLog.Log("StoreCenter", "check if dates are OK");
                if ((startDate< MarketYard.MarketDate)|| (endDate < MarketYard.MarketDate) || !(startDate < endDate))
                {
                    MarketLog.Log("StoreCenter", "something wrong with the dates");
                    throw new StoreException(DiscountStatus.DatesAreWrong, "dates are not leagal");
                }
                MarketLog.Log("StoreCenter", "check that discount amount is OK");
                if (presenteges && (discountAmount >= 100))
                {
                    MarketLog.Log("StoreCenter", "discount amount is >=100%");
                    throw new StoreException(DiscountStatus.AmountIsHundredAndpresenteges, "DiscountAmount is >= 100%");
                }
                if (discountAmount <= 0)
                {
                    MarketLog.Log("StoreCenter", "discount amount <=0");
                    throw new StoreException(DiscountStatus.discountAmountIsNegativeOrZero, "DiscountAmount is >= 100%");
                }
                if (!presenteges && (discountAmount > product.BasePrice))
                {
                    MarketLog.Log("StoreCenter", "discount amount is >= product price");
                    throw new StoreException(DiscountStatus.DiscountGreaterThenProductPrice, "DiscountAmount is > then product price");
                }
                StockListItem stockListItem = global.DataLayer.GetProductFromStore(_storeName, product.Name);
                MarketLog.Log("StoreCenter", "check that the product don't have another discount");
                if (stockListItem.Discount != null)
                {
                    MarketLog.Log("StoreCenter", "the product have another discount");
                    throw new StoreException(DiscountStatus.thereIsAlreadyAnotherDiscount, "the product have another discount");
                }
                Discount discount = new Discount(global.GetDiscountCode(), global.GetdiscountTypeEnumString(discountType), startDate,
                    endDate, discountAmount, presenteges);
                stockListItem.Discount = discount;
                global.DataLayer.AddDiscount(discount);
                discountsToRemvoe.AddLast(discount);
                global.DataLayer.EditStockInDatabase(stockListItem);
                MarketLog.Log("StoreCenter", "discount added successfully");
                return new StoreAnswer(DiscountStatus.Success, "discount added successfully");
            }
            catch (StoreException exe)
            {
                return new StoreAnswer(exe);
            }
            catch (MarketException)
            {
                return new StoreAnswer(StoreEnum.NoPremmision, "you have no premmision to do that");
            }
        }
        public MarketAnswer EditDiscount(string productName, string whatToEdit, string newValue)
        {

            StoreAnswer result = null;
            try
            {
                MarketLog.Log("StoreCenter", "trying to edit discount from product in store");
                checkIfStoreExists();
                MarketLog.Log("StoreCenter", " check if has premmision to edit products");
                _storeManager.CanDeclareDiscountPolicy();
                MarketLog.Log("StoreCenter", " has premmission");
                checkIfProductExists(productName);
                Discount discount = checkIfDiscountExistsPrivateMethod(productName);
                if ((whatToEdit == "discountType") || (whatToEdit == "DiscountType") || (whatToEdit == "discounttype") || (whatToEdit == "DISCOUNTTYPE"))
                {
                    discount = editDiscountdiscyoutTypePrivateMethod(discount, newValue, out result, productName);

                }
                if ((whatToEdit == "startDate") || (whatToEdit == "start Date") || (whatToEdit == "StartDate") || (whatToEdit == "Start Date") || (whatToEdit == "startdate") || (whatToEdit == "start date") || (whatToEdit == "STARTDATE") || (whatToEdit == "START DATE"))
                {
                    discount = editDiscountStartDatePrivateMethod(discount, newValue, out result, productName);
                }

                if ((whatToEdit == "EndDate") || (whatToEdit == "end Date") || (whatToEdit == "enddate") || (whatToEdit == "End Date") || (whatToEdit == "end date") || (whatToEdit == "ENDDATE") || (whatToEdit == "END DATE"))
                {
                    discount = editDiscountEndDatePrivateMethod(discount, newValue, out result, productName);
                }

                if ((whatToEdit == "DiscountAmount") || (whatToEdit == "Discount Amount") || (whatToEdit == "discount amount") || (whatToEdit == "discountamount") || (whatToEdit == "DISCOUNTAMOUNT") || (whatToEdit == "DISCOUNT AMOUNT"))
                {
                    discount = editDiscountDiscountAmountPrivateMehtod(discount, newValue, out result, productName);
                }
                if ((whatToEdit == "Percentages") || (whatToEdit == "percentages") || (whatToEdit == "PERCENTAGES"))
                {
                    discount = editDiscountPercentagesPrivateMehtod(discount, newValue, out result, productName);
                }
                if (result == null) { throw new StoreException(DiscountStatus.NoLegalAttrebute, "no leagal attrebute found"); }
                global.DataLayer.EditDiscountInDatabase(discount);
                return result;
            }
            catch (StoreException exe)
            {
                return new StoreAnswer(exe);
            }
            catch (MarketException)
            {
                return new StoreAnswer(StoreEnum.NoPremmision, "you have no premmision to do that");
            }
        }


        public MarketAnswer RemoveDiscountFromProduct(string productName)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to remove discount from product in store");
                MarketLog.Log("StoreCenter", "check if store exists");
                checkIfStoreExists();
                MarketLog.Log("StoreCenter", " check if has premmision to edit products");
                _storeManager.CanDeclareDiscountPolicy();
                MarketLog.Log("StoreCenter", " has premmission");
                checkIfProductExists(productName);
                Discount D = checkIfDiscountExistsPrivateMethod(productName);
                StockListItem stockListItem = global.GetProductFromStore(_storeName, productName);
                stockListItem.Discount = null;
                global.DataLayer.RemoveDiscount(D);
                global.DataLayer.EditStockInDatabase(stockListItem);
                MarketLog.Log("StoreCenter", "discount removed successfully");
                return new StoreAnswer(DiscountStatus.Success, "discount removed successfully");
            }
            catch (StoreException exe)
            {
                return new StoreAnswer(exe);
            }
            catch (MarketException)
            {
                return new StoreAnswer(StoreEnum.NoPremmision, "you have no premmision to do that");
            }
        }
        public MarketAnswer ViewStoreHistory()
        {
            MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " attempting to view the store purchase history...");
            try
            {
                global.DataLayer.IsStoreExistAndActive(_storeName);
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
                LinkedList<string> items = global.DataLayer.GetAllStoreProductsID(store.SystemId);
                foreach (string id in items)
                {
                    StockListItem item = global.DataLayer.GetStockListItembyProductID(id);
                    global.DataLayer.RemoveStockListItem(item);
                }
                global.DataLayer.RemoveStore(store);
            }
        }

        public MarketAnswer AddQuanitityToProduct(string productName, int quantity)
        {
            try
            {
                MarketLog.Log("StoreCenter", "checking that store exists");
                if (!global.DataLayer.IsStoreExist(_storeName))
                {
                    MarketLog.Log("StoreCenter", "store not exists");
                    return new StoreAnswer(StoreEnum.StoreNotExists, "store not exists");
                }
                MarketLog.Log("StoreCenter", "checking that has premmisions");
                _storeManager.CanManageProducts();
                MarketLog.Log("StoreCenter", "checking that Product Exists");
                if (global.IsProductNameAvailableInStore(_storeName, productName))
                {
                    MarketLog.Log("StoreCenter", "Product Not exists");
                    return new StoreAnswer(StoreEnum.ProductNotFound, "product not exists");
                }
                StockListItem stockListItem = global.DataLayer.GetProductFromStore(_storeName, productName);
                MarketLog.Log("StoreCenter", "checking that quantity is positive");
                if (quantity <= 0)
                {
                    MarketLog.Log("StoreCenter", "quantity is not positive");
                    return new StoreAnswer(StoreEnum.quantityIsNegatie, "quantity " + quantity + " is less then or equal to 0");
                }
                stockListItem.Quantity += quantity;
                global.DataLayer.EditStockInDatabase(stockListItem);
                return new StoreAnswer(StoreEnum.Success, "item " + productName + " added by amound of " + quantity);
            }
            catch (MarketException e)
            {
                MarketLog.Log("StoreCenter", "Manager " + _storeManager.GetID() + " has no permission to view purchase history in Store"
                                             + _storeName + " and therefore has been denied. Error message has been created!");
                return new StoreAnswer(StoreEnum.NoPremmision, e.GetErrorMessage());
            }
        }
        private Discount checkIfDiscountExistsPrivateMethod(string productName)
        {
            StockListItem stockListItem = global.GetProductFromStore(_storeName, productName);
            MarketLog.Log("StoreCenter", " Product exists");
            MarketLog.Log("StoreCenter", "checking that the product has a discount");
            Discount discount = stockListItem.Discount;
            if (discount == null)
            {
                MarketLog.Log("StoreCenter", "product does not exists");
                throw new StoreException(DiscountStatus.DiscountNotFound, "there is no discount at this product");
            }
            MarketLog.Log("StoreCenter", " check what you want to edit");
            return discount;
        }
        private void checkIfProductExists(string productName)
        {
            MarketLog.Log("StoreCenter", " check if product name exists in the store " + store.Name);
            if (global.IsProductNameAvailableInStore(_storeName, productName))
            {
                MarketLog.Log("StoreCenter", "product does not exists");
                throw new StoreException(DiscountStatus.ProductNotFound, "product not found");
            }
        }
        private Discount editDiscountdiscyoutTypePrivateMethod(Discount discount, string newValue, out StoreAnswer result, string productName)
        {
            MarketLog.Log("StoreCenter", " edit discount type");
            discount.discountType = global.GetdiscountTypeEnumString(newValue);
            MarketLog.Log("StoreCenter", " discount type changed successfully");
            result = new StoreAnswer(StoreEnum.Success, "item " + productName + " discount type become " + newValue);
            return discount;
        }
        private void checkIfStoreExists()
        {
            MarketLog.Log("StoreCenter", "check if store exists");
            if (!global.DataLayer.IsStoreExist(_storeName))
            {
                MarketLog.Log("StoreCenter", " store does not exists");
                throw new StoreException(DiscountStatus.NoStore, "store not exists");
            }
            MarketLog.Log("StoreCenter", " store exists");
        }
        private Discount editDiscountStartDatePrivateMethod(Discount discount, string newValue, out StoreAnswer result, string productName)
        {

            MarketLog.Log("StoreCenter", " edit start date");
            MarketLog.Log("StoreCenter", " checking that the start date is legal");
            DateTime startTime = DateTime.MaxValue;
            if (!DateTime.TryParse(newValue, out startTime))
            {
                MarketLog.Log("StoreCenter", "date format is not legal");
                throw new StoreException(DiscountStatus.DatesAreWrong, "date format is not legal");
            }
            if (startTime.Date < MarketYard.MarketDate)
            {
                MarketLog.Log("StoreCenter", "can't set start time in the past");
                throw new StoreException(DiscountStatus.DatesAreWrong, "can't set start time in the past");
            }

            if (startTime.Date >= discount.EndDate.Date)
            {
                MarketLog.Log("StoreCenter", "can't set start time that is later then the discount end time");
                throw new StoreException(DiscountStatus.DatesAreWrong, "can't set start time that is later then the discount end time");
            }
            discount.startDate = startTime;
            MarketLog.Log("StoreCenter", " start date changed successfully");
            result = new StoreAnswer(StoreEnum.Success, "item " + productName + " discount Start Date become " + startTime);
            return discount;
        }
        private Discount editDiscountPercentagesPrivateMehtod(Discount discount, string newValue, out StoreAnswer result, string productName)
        {
            MarketLog.Log("StoreCenter", "try to edit precenteges");
            bool newboolValue = true;
            if (!Boolean.TryParse(newValue, out newboolValue))
            {
                MarketLog.Log("StoreCenter", "value is not legal");
                throw new StoreException(DiscountStatus.precentegesIsNotBoolean, "value is not legal");
            }
            MarketLog.Log("StoreCenter", "checking that the discount amount is fit to precenteges");
            if ((newboolValue) && (discount.DiscountAmount >= 100))
            {
                MarketLog.Log("StoreCenter", "DiscountAmount is >= 100, cant make it presenteges");
                throw new StoreException(DiscountStatus.AmountIsHundredAndpresenteges, "DiscountAmount is >= 100, cant make it presenteges");
            }
            discount.Percentages = newboolValue;
            if (newboolValue)
                result = new StoreAnswer(StoreEnum.Success, "item " + productName + " discount preseneges become true");
            result = new StoreAnswer(StoreEnum.Success, "item " + productName + " discount preseneges become false");
            return discount;
        }
        private Discount editDiscountDiscountAmountPrivateMehtod(Discount discount, string newValue, out StoreAnswer result, string productName)
        {

            MarketLog.Log("StoreCenter", " edit discount amount");
            int newintValue = 0;
            if (!Int32.TryParse(newValue, out newintValue))
            {
                MarketLog.Log("StoreCenter", "value is not legal");
                throw new StoreException(DiscountStatus.discountAmountIsNotNumber, "value is not legal");
            }
            if ((discount.Percentages) && (newintValue >= 100))
            {
                MarketLog.Log("StoreCenter", "DiscountAmount is >= 100, cant make it presenteges");
                throw new StoreException(DiscountStatus.AmountIsHundredAndpresenteges, "DiscountAmount is >= 100, cant make it presenteges");
            }
            if ((!discount.Percentages) && (newintValue > global.DataLayer.getProductByNameFromStore(_storeName, productName).BasePrice))
            {
                MarketLog.Log("StoreCenter", "discount amount is >= product price");
                throw new StoreException(DiscountStatus.DiscountGreaterThenProductPrice, "DiscountAmount is > then product price");
            }
            if (newintValue <= 0)
            {
                MarketLog.Log("StoreCenter", "discount amount <=0");
                throw new StoreException(DiscountStatus.discountAmountIsNegativeOrZero, "DiscountAmount is >= 100%");
            }
            discount.DiscountAmount = newintValue;
            MarketLog.Log("StoreCenter", "discount amount set to " + newintValue);
            result = new StoreAnswer(StoreEnum.Success, "item " + productName + " discount amount become " + newValue);
            return discount;
        }
        private Discount editDiscountEndDatePrivateMethod(Discount discount, string newValue, out StoreAnswer result, string productName)
        {

            MarketLog.Log("StoreCenter", " edit start date");
            MarketLog.Log("StoreCenter", " checking that the start date is legal");
            DateTime EndDate = DateTime.MaxValue;
            if (!DateTime.TryParse(newValue, out EndDate))
            {
                MarketLog.Log("StoreCenter", "date format is not legal");
                throw new StoreException(DiscountStatus.DatesAreWrong, "date format is not legal");
            }
            if (EndDate.Date < MarketYard.MarketDate)
            {
                MarketLog.Log("StoreCenter", "can't set end time in the past");
                throw new StoreException(DiscountStatus.DatesAreWrong, "can't set end time in the past");
            }
            if (EndDate.Date < discount.startDate.Date)
            {
                MarketLog.Log("StoreCenter", "can't set end time that is sooner then the discount start time");
                throw new StoreException(DiscountStatus.DatesAreWrong, "can't set end time that is sooner then the discount start time");
            }
            discount.EndDate = EndDate;
            MarketLog.Log("StoreCenter", " start date changed successfully");
            result = new StoreAnswer(StoreEnum.Success, "item " + productName + " discount End Date become " + EndDate);
            return discount;
        }



    }
}
