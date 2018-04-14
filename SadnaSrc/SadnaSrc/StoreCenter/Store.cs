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
     * this class is describing a single store, the managmnet of all the stores + implementing StoreService is done in StoreCenter
     **/
    public class Store
    {
        public string SystemId { get; }
        private LinkedList<PurchasePolicy> PurchasePolicy;
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public Store(string id, string name, string address)
        {
            SystemId = id;
            Name = name;
            Address = address;
            PurchasePolicy = new LinkedList<PurchasePolicy>();
            IsActive = true;
        }

        public Store(string id, string name, string address, string active)
        {
            SystemId = id;
            Name = name;
            Address = address;
            PurchasePolicy = new LinkedList<PurchasePolicy>();
            GetActiveFromString(active);
        }

        private void GetActiveFromString(string active)
        {
            if (active.Equals("Active"))
                IsActive = true;
            IsActive = false;
        }

        public string GetStringFromActive()
        {
            return IsActive ? "Active" : "InActive";
        }
        public MarketAnswer CloseStore()
        {
            if (IsActive)
            {
                IsActive = false;
                ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
                handler.DataLayer.EditStore(this);
                return new StoreAnswer(StoreEnum.Success, "store " + SystemId + " closed");
            }
            return new StoreAnswer(StoreEnum.CloseStoreFail, "store " + SystemId + " is already closed");
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(this.GetType()))
            {
                return ((Store)obj).SystemId.Equals(SystemId) &&
                    ((Store)obj).Name.Equals(Name) &&
                    ((Store)obj).Address.Equals(Address);
            }
            return false;

        }
    }
}
        //////////////////// this function will be removed after I will have Maor function!//////////////////////
/*
        

   
        
        public Product GetProductById(string ID) //will return null if product is not exists
        {
            return stock.GetProductById(ID);
        }
        
        internal LinkedList<Product> AddAllProductsToExistingList(LinkedList<Product> param)
        {
            LinkedList<Product> answer = new LinkedList<Product>();
            foreach (Product product in param)
            {
                answer.AddLast(product);
            }
            LinkedList<Product> products = GetAllProducts();
            foreach (Product product in products)
            {
                answer.AddLast(product);
            }
            return answer;
        }
        private LotterySaleManagmentTicket GetLotterySale(Product p)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            return handler.DataLayer.GetLotteryByProductID(p.SystemId);
        }
        public LotteryTicket MakeALotteryPurchase(string productID, int money, int userID)
        {
            LotteryTicket result = null;
            Product product = stock.GetProductById(productID);
            if (product == null) return null;
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            LotterySaleManagmentTicket lotteryManagement = handler.DataLayer.GetLotteryByProductID(productID);
            if (CanPurchaseLottery(product, money))
            {
                result = lotteryManagement.PurchaseALotteryTicket(money, userID);
            }
            return result;
         }
        
        public Product MakeAImmediatePurchase(string productID, int quantity)
        {
            Product product = stock.GetProductById(productID);
            if (product == null) return null;
            if (CanPurchaseImmediate(product, quantity))
            {
                return product;
            }
            return null;
        }
        internal bool CanPurchaseImmediate(Product product, int quantity)
        {
            StockListItem stockListItem = stock.FindstockListItembyProductID(product.SystemId);
            return (stockListItem.Quantity >= quantity);
        }
        internal bool CanPurchaseLottery(Product product, int amountOfMoney)
        {
            StockListItem stockListItem = stock.FindstockListItembyProductID(product.SystemId);
            return stockListItem.PurchaseWay == PurchaseEnum.Lottery &&
                    stockListItem.Quantity > 0 &&
                    GetLotterySale(product) != null &&
                    GetLotterySale(product).CanPurchase(amountOfMoney);
        }



        public MarketAnswer EditDiscount(string productID, string whatToEdit, string newValue)
        {
            StoreAnswer result = null;
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            Product product = stock.GetProductById(productID);
            if (product == null) { return new StoreAnswer(StoreEnum.ProductNotFound, "product " + productID + " does not exist in Stock"); };
                Discount discount = stock.GetProductDiscount(product);
            if (discount == null) { return new StoreAnswer(StoreEnum.DiscountNotFound, "product " + product.ToString() + " has no discount"); };

            if (whatToEdit == "discountType")
            {
                discount.discountType = handler.GetdiscountTypeEnumString(newValue);
                result = new StoreAnswer(StoreEnum.Success, "item " + product.ToString() + " discount type become " +newValue);
            }
            if (whatToEdit == "startDate")
            {
                DateTime startTime = DateTime.Parse(newValue);
                if (startTime < 
                
    
    
    
    
    .Date) { return new StoreAnswer(DiscountStatus.DatesAreWrong, "can't set start time in the past"); }

                if (startTime > discount.EndDate) { return new StoreAnswer(DiscountStatus.DatesAreWrong, "can't set start time that is later then the discount end time"); }
                discount.startDate = startTime;
                result= new StoreAnswer(StoreEnum.Success, "item " + product.ToString() + " discount Start Date become " + startTime);
            }
            
            if (whatToEdit == "EndDate")
            {
                DateTime EndDate = DateTime.Parse(newValue);
                if (EndDate < DateTime.Now.Date) { return new StoreAnswer(StoreEnum.UpdateStockFail, "can't set start time in the past"); }

                if (EndDate < discount.startDate) { return new StoreAnswer(StoreEnum.UpdateStockFail, "can't set end time that is sooner then the discount start time"); }
                discount.EndDate = EndDate;
                result = new StoreAnswer(StoreEnum.Success, "item " + product.ToString() + " discount End Date become " + EndDate);
            }
                
            if (whatToEdit == "DiscountAmount")
            {
               int newintValue = Int32.Parse(newValue);
               if (discount.Percentages && newintValue > 100) { return new StoreAnswer(StoreEnum.UpdateStockFail, "DiscountAmount is >= 100, cant make it presenteges"); }
                discount.DiscountAmount = newintValue;
               return new StoreAnswer(StoreEnum.Success, "item " + product.ToString() + " discount amount become " + newValue);
            }
            if (whatToEdit == "Percentages")
            {
                bool newboolValue = Boolean.Parse(newValue);
                if (newboolValue && discount.DiscountAmount > 100) { return new StoreAnswer(StoreEnum.UpdateStockFail, "DiscountAmount is >= 100, cant make it presenteges"); }
                discount.Percentages = newboolValue;
                if (newboolValue)
                    result = new StoreAnswer(StoreEnum.Success, "item " + product.ToString() + " discount preseneges become true");
                result = new StoreAnswer(StoreEnum.Success, "item " + product.ToString() + " discount preseneges become false");
            }
            if (result==null) { return new StoreAnswer(StoreEnum.UpdateStockFail, "no leagal attrebute found"); }
            handler.DataLayer.EditDiscountInDatabase(discount);
            return result;
        }

        internal MarketAnswer SetStoreAddress(string _address)
        {
            Address = _address;
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            handler.DataLayer.EditStore(this);
            return new StoreAnswer(StoreEnum.Success, "Store Address changed");
        }

        internal MarketAnswer SetStoreName(string _name)
        {
            Address = _name;
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            handler.DataLayer.EditStore(this);
            return new StoreAnswer(StoreEnum.Success, "Store Address changed");
        }

        public MarketAnswer EditProduct (string productID, string whatToEdit, string newValue)
        {
            StoreAnswer result = null;
            Product product = stock.GetProductById(productID);
            if (product==null) { return new StoreAnswer(StoreEnum.ProductNotFound, "product " + productID + " does not exist in Stock"); }
            if (whatToEdit == "Name") {
                
                result = new StoreAnswer(StoreEnum.Success, "product " + product.SystemId + " name has been updated to " + newValue);
                product.Name = newValue;
            }
            if (whatToEdit == "BasePrice") {
                int newBasePrice = Int32.Parse(newValue);
                if (newBasePrice < 0) { return new StoreAnswer(StoreEnum.UpdateProductFail, "price can not be negative"); };
                result = new StoreAnswer(StoreEnum.Success, "product " + product.SystemId + " price has been updated to " + newValue);
                product.BasePrice = Int32.Parse(newValue);
            }
            if (whatToEdit == "Desccription") {
                result = new StoreAnswer(StoreEnum.Success, "product " + product.SystemId + " Description has been updated to " + newValue);
                product.Description = newValue;
            }
            if (result==null) { return new StoreAnswer(StoreEnum.UpdateProductFail, "no leagal attrebute found"); }
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            handler.DataLayer.EditProductInDatabase(product);
            return result;
        }
         public MarketAnswer EditStockListItem(string productID, string whatToEdit, string newValue)
        {
            StoreAnswer result = null;
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            StockListItem stockListItem = stock.FindstockListItembyProductID(productID);
            if (stockListItem==null) { return new StoreAnswer(StoreEnum.ProductNotFound, "product " + productID + " does not exist in Stock"); }
            if (whatToEdit == "quantity")
            {
                int quantity = Int32.Parse(newValue);
                if (quantity<0) { return new StoreAnswer(StoreEnum.UpdateStockFail, "quantity " + quantity + " is less then 0"); }
                stockListItem.Quantity += quantity;
                result = new StoreAnswer(StoreEnum.Success, "item " + productID + " added by amound of " + quantity);
            }
            if (whatToEdit == "PurchaseWay")
            {
                PurchaseEnum purchaseEnum = handler.GetPurchaseEnumString(newValue);
                if (purchaseEnum.Equals(null)) { return new StoreAnswer(StoreEnum.UpdateStockFail, "this Purchase Enum is not leagal"); }
                if (stockListItem.PurchaseWay == PurchaseEnum.Lottery)
                {
                    LotterySaleManagmentTicket lotteryManagment = handler.DataLayer.GetLotteryByProductID(productID);
                    lotteryManagment.InformCancel();
                    handler.DataLayer.EditLotteryInDatabase(lotteryManagment);
                }
                if (purchaseEnum == PurchaseEnum.Lottery)
                {
                    LotterySaleManagmentTicket lotteryManagment = new LotterySaleManagmentTicket(handler.GetLottyerID(), Name, stockListItem.Product, DateTime.Now, DateTime.MaxValue);
                    handler.DataLayer.AddLottery(lotteryManagment);
                    stockListItem.PurchaseWay = purchaseEnum;
                    result = new StoreAnswer(StoreEnum.Success, "item " + stockListItem.Product.SystemId + " added PurchaseWay of Lottery, yet, you should change it's values");
                }
                else { 
                stockListItem.PurchaseWay = purchaseEnum;
                result = new StoreAnswer(StoreEnum.Success, "item " + stockListItem.Product.SystemId + " added PurchaseWay of" + newValue);

                }
            }
            if (result == null) { return new StoreAnswer(StoreEnum.UpdateProductFail, "no leagal attrebute found"); }
            handler.DataLayer.EditStockInDatabase(stockListItem);
            return result;
        }
        internal LinkedList<Product> GetAllProducts()
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            LinkedList<string> theirID = handler.DataLayer.GetAllStoreProductsID(SystemId);
            LinkedList<Product> result = new LinkedList<Product>();
            foreach (string ID in theirID)
            {
                Product product = handler.DataLayer.GetProductID(ID);
                    result.AddLast(product);
            }
            return result;
        }
        
        internal MarketAnswer RemoveDiscountFromProduct(string productID)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            StockListItem stockListItem = stock.FindstockListItembyProductID(productID);
            if (stockListItem == null) return new StoreAnswer(StoreEnum.ProductNotFound, "product " + productID + " does not exist in Stock");
            Discount discount = stockListItem.Discount;
            handler.DataLayer.RemoveDiscount(discount);
            stockListItem.Discount = null;
            handler.DataLayer.EditStockInDatabase(stockListItem);
            return new StoreAnswer(StoreEnum.Success, "discount remvoed");
        }
        internal void UpdateQuanityAfterPurchase(Product product, int quantity)
        {
            StockListItem stockListItem = stock.FindstockListItembyProductID(product.SystemId);
            if (stockListItem == null) throw new StoreException(StoreSyncStatus.NoProduct, "Item not found");
            stockListItem.Quantity = stockListItem.Quantity - quantity;
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            handler.DataLayer.EditStockInDatabase(stockListItem);
        }
        
        public LotteryTicket DoLottery(string product)
        {
            throw new NotImplementedException();
        }
        internal bool IsOwner(User user) //waiting to Maor's function
        {
            return true;
        }
        public string[] ViewPurchesHistory()
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            return handler.DataLayer.GetHistory(this);
        }
        
}

*/