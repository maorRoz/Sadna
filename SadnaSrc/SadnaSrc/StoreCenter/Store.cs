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
        private Stock stock { get; set; }
        private LinkedList<PurchasePolicy> PurchasePolicy;
        private LinkedList<String> history;
        private bool isActive { get; set; }

        public Store(User _Owner, string id)
        {
            SystemId = id;
            stock = new Stock(SystemId);
            PurchasePolicy = new LinkedList<PurchasePolicy>();
            isActive = true;
        }

        public bool IsStoreActive()
        {
            return isActive;
        }
        ////////////////////  this is a proxy function  /////////////////////////////////////////////////////////
        public bool IsOwner(User user)
        {
            return true;
        }
        //////////////////// this function will be removed after I will have Maor function!//////////////////////


        public MarketAnswer PromoteToManager(User currentUser, User someoneToPromote)
        {    
               return new StoreAnswer(StoreEnum.Success, "user " + someoneToPromote + " has been premoted to be a owner of store " + SystemId);
        }


        public MarketAnswer CloseStore()
        {
                if (isActive)
                {
                    isActive = false;
                    return new StoreAnswer(StoreEnum.Success, "store " + SystemId + " closed");
                }
                return new StoreAnswer(StoreEnum.Success, "store " + SystemId + " is alrady closed");
        }

        
        public MarketAnswer AddProduct(string _name, int _price, string _description, int quantity)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
            Product P = new Product(handler.getProductID(), _name, _price, _description);
            handler.dataLayer.AddStockListItemToDataBase(new Stock.StockListItem(quantity, P, null, PurchaseEnum.IMMEDIATE));
            handler.dataLayer.AddProductToDatabase(P);
            return new StoreAnswer(StoreEnum.Success, "product added");
        }

        public MarketAnswer removeProduct(string productID)
        {
            Product product = stock.getProductById(productID);
            if (product==null) { return new StoreAnswer(StoreEnum.ProductNotFound, "no Such Product"); }
            ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
            Stock.StockListItem SLI = handler.dataLayer.getStockListItembyProductID(productID);
            if (SLI.PurchaseWay==PurchaseEnum.LOTTERY)
            {
                LotterySaleManagmentTicket LSMT = handler.dataLayer.getLotteryByProductID(productID);
                LSMT.informCancel();
                handler.dataLayer.removeLottery(LSMT);
            }
            handler.dataLayer.removeDiscount(SLI.discount);
            handler.dataLayer.removeProduct(SLI.product);
            handler.dataLayer.removeStockListItem(SLI);
            return new StoreAnswer(StoreEnum.Success, "product removed");
        }

        internal double getProductPriceWithDiscountbyDouble(string productName, int discountCode, int quantity)
        {
            Product p = stock.getProductById(productName);
            if (p != null) { 
            return stock.CalculateSingleItemPrice(p, discountCode, quantity);
            }

            return -1;
        }


        public Product getProductById(string ID) //will return null if product is not exists
        {
            return stock.getProductById(ID);
        }
        
        internal void addAllProductsToExistingList(LinkedList<Product> result)
        {
            LinkedList<Product> products = getAllProducts();
            foreach (Product product in products)
            {
                result.AddLast(product);
            }
        }
        private LotterySaleManagmentTicket getLotterySale(Product p)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
            LotterySaleManagmentTicket LSMT = handler.dataLayer.getLotteryByProductID(p.SystemId);
            return LSMT;
        }
        public LotteryTicket MakeALotteryPurchase(string productID, int money)
        {
            LotteryTicket result = null;
            Product product = stock.getProductById(productID);
            if (product == null) return null;
            ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
            LotterySaleManagmentTicket LSMT = handler.dataLayer.getLotteryByProductID(productID);
            if (canPurchaseLottery(product, money))
            {
                result = LSMT.PurchaseALotteryTicket(money);

            }
            return result;
         }
        
        public Product MakeAImmediatePurchase(string productID, int quantity)
        {
            Product product = stock.getProductById(productID);
            if (product == null) return null;
            if (canPurchaseImmediate(product, quantity))
            {
                return product;
            }
            return null;
        }
        public MarketAnswer getProductPriceWithDiscount(string _product, int _DiscountCode, int _quantity)
        {
            double result = stock.CalculateSingleItemPrice(stock.getProductById(_product), _DiscountCode, _quantity);
            return new StoreAnswer(StoreEnum.Success, "" + result);
        }
        internal bool canPurchaseImmediate(Product product, int quantity)
        {
            Stock.StockListItem SLI = stock.findstockListItembyProductID(product.SystemId);
            return (SLI.quantity >= quantity);
        }
        internal bool canPurchaseLottery(Product product, int amountOfMoney)
        {
            Stock.StockListItem SLI = stock.findstockListItembyProductID(product.SystemId);
            return ((SLI.PurchaseWay == PurchaseEnum.LOTTERY) &&
                    (SLI.quantity > 0) &&
                    (getLotterySale(product) != null) &&
                    (getLotterySale(product).CanPurchase(amountOfMoney)));
        }



        public MarketAnswer EditDiscount(string productID, string whatToEdit, string newValue)
        {
            StoreAnswer result = null;
            ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
            Product product = stock.getProductById(productID);
            if (product == null) { return new StoreAnswer(StoreEnum.ProductNotFound, "product " + productID + " does not exist in Stock"); };
                Discount discount = stock.getProductDiscount(product);
            if (discount == null) { return new StoreAnswer(StoreEnum.DiscountNotFound, "product " + product.toString() + " has no discount"); };

            if (whatToEdit == "discountType")
            {
                discount.discountType = handler.GetdiscountTypeEnumString(newValue);
                result = new StoreAnswer(StoreEnum.Success, "item " + product.toString() + " discount type become " +newValue);
            }
            if (whatToEdit == "startDate")
            {
                DateTime startTime = DateTime.Parse(newValue);
                if (startTime < DateTime.Now.Date) { return new StoreAnswer(StoreEnum.UpdateDiscountFail, "can't set start time in the past"); }

                if (startTime > discount.EndDate) { return new StoreAnswer(StoreEnum.UpdateDiscountFail, "can't set start time that is later then the discount end time"); }
                discount.startDate = startTime;
                result= new StoreAnswer(StoreEnum.Success, "item " + product.toString() + " discount Start Date become " + startTime);
            }
            
            if (whatToEdit == "EndDate")
            {
                DateTime EndDate = DateTime.Parse(newValue);
                if (EndDate < DateTime.Now.Date) { return new StoreAnswer(StoreEnum.UpdateDiscountFail, "can't set start time in the past"); }

                if (EndDate < discount.startDate) { return new StoreAnswer(StoreEnum.UpdateDiscountFail, "can't set end time that is sooner then the discount start time"); }
                discount.EndDate = EndDate;
                result = new StoreAnswer(StoreEnum.Success, "item " + product.toString() + " discount End Date become " + EndDate);
            }
                
            if (whatToEdit == "DiscountAmount")
            {
               int newintValue = Int32.Parse(newValue);
               if (discount.Percentages && newintValue > 100) { return new StoreAnswer(StoreEnum.UpdateDiscountFail, "DiscountAmount is >= 100, cant make it presenteges"); }
                discount.DiscountAmount = newintValue;
               return new StoreAnswer(StoreEnum.Success, "item " + product.toString() + " discount amount become " + newValue);
            }
            if (whatToEdit == "Percentages")
            {
                bool newboolValue = Boolean.Parse(newValue);
                if (newboolValue && discount.DiscountAmount > 100) { return new StoreAnswer(StoreEnum.UpdateDiscountFail, "DiscountAmount is >= 100, cant make it presenteges"); }
                discount.Percentages = newboolValue;
                if (newboolValue)
                    result = new StoreAnswer(StoreEnum.Success, "item " + product.toString() + " discount preseneges become true");
                result = new StoreAnswer(StoreEnum.Success, "item " + product.toString() + " discount preseneges become false");
            }
            if (result==null) { return new StoreAnswer(StoreEnum.UpdateDiscountFail, "no leagal attrebute found"); }
            handler.dataLayer.EditDiscountInDatabase(discount);
            return result;
        }
        public MarketAnswer EditProduct (string productID, string whatToEdit, string newValue)
        {
            StoreAnswer result = null;
            Product product = stock.getProductById(productID);
            if (product==null) { return new StoreAnswer(StoreEnum.ProductNotFound, "product " + productID + " does not exist in Stock"); }
            if (whatToEdit == "Name") {
                result = new StoreAnswer(StoreEnum.Success, "product " + product.SystemId + " name has been updated to " + newValue);
                product.name = newValue;
            }
            if (whatToEdit == "BasePrice") {
                int newBasePrice = Int32.Parse(newValue);
                if (newBasePrice < 0) { return new StoreAnswer(StoreEnum.UpdateProductFail, "price can not be negative"); };
                result = new StoreAnswer(StoreEnum.Success, "product " + product.SystemId + " price has been updated to " + newValue);
                product.BasePrice = Int32.Parse(newValue);
            }
            if (whatToEdit == "Desccription") {
                result = new StoreAnswer(StoreEnum.Success, "product " + product.SystemId + " Description has been updated to " + newValue);
                product.description = newValue;
            }
            if (result==null) { return new StoreAnswer(StoreEnum.UpdateProductFail, "no leagal attrebute found"); }
            ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
            handler.dataLayer.EditProductInDatabase(product);
            return result;
        }
         public MarketAnswer editStockListItem(string productID, string whatToEdit, string newValue)
        {
            StoreAnswer result = null;
            ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
            Stock.StockListItem stockListItem = stock.findstockListItembyProductID(productID);
            if (stockListItem==null) { return new StoreAnswer(StoreEnum.ProductNotFound, "product " + productID + " does not exist in Stock"); }
            if (whatToEdit == "quantity")
            {
                int quantity = Int32.Parse(newValue);
                if (quantity<0) { return new StoreAnswer(StoreEnum.UpdateStockFail, "quantity " + quantity + " is less then 0"); }
                stockListItem.quantity += quantity;
                result = new StoreAnswer(StoreEnum.Success, "item " + productID + " added by amound of " + quantity);
            }
            if (whatToEdit == "PurchaseWay")
            {
                PurchaseEnum purchaseEnum = handler.GetPurchaseEnumString(newValue);

                if (stockListItem.PurchaseWay == PurchaseEnum.LOTTERY)
                {
                    LotterySaleManagmentTicket LSMT = handler.dataLayer.getLotteryByProductID(productID);
                    LSMT.informCancel();
                    handler.dataLayer.editLotteryInDatabase(LSMT);
                }
                if (purchaseEnum == PurchaseEnum.LOTTERY)
                {
                    LotterySaleManagmentTicket LSMT2 = new LotterySaleManagmentTicket(handler.getLottyerID(), stockListItem.product, DateTime.Now, DateTime.MaxValue);
                    handler.dataLayer.AddLottery(LSMT2);
                    stockListItem.PurchaseWay = purchaseEnum;
                    result = new StoreAnswer(StoreEnum.Success, "item " + stockListItem.product.SystemId + " added PurchaseWay of Lottery, yet, you should change it's values");
                }
                else { 
                stockListItem.PurchaseWay = purchaseEnum;
                result = new StoreAnswer(StoreEnum.Success, "item " + stockListItem.product.SystemId + " added PurchaseWay of" + newValue);

                }
            }
            if (result == null) { return new StoreAnswer(StoreEnum.UpdateProductFail, "no leagal attrebute found"); }
            handler.dataLayer.EditStockInDatabase(stockListItem);
            return result;
        }
        internal LinkedList<Product> getAllProducts()
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
            LinkedList<string> theirID = handler.dataLayer.getAllStoreProductsID(SystemId);
            LinkedList<Product> result = new LinkedList<Product>();
            foreach (string ID in theirID)
            {
                Product product = handler.dataLayer.getProductID(ID);
                    result.AddLast(product);
            }
            return result;
        }
        internal MarketAnswer getProductStockInformation(string productID)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
            Stock.StockListItem SLI = stock.findstockListItembyProductID(productID);
            if (SLI==null) return new StoreAnswer(StoreEnum.ProductNotFound, "product " + productID + " does not exist in Stock");
            string Product = SLI.product.toString();
            string Discount = SLI.discount.toString();
            string PurchaseWay = handler.PrintEnum(SLI.PurchaseWay);
            string Quanitity = SLI.quantity + "";
            string result = Product + " , " + Discount + " , " + PurchaseWay + " , " + Quanitity;
            return new StoreAnswer(StoreEnum.Success, result);
        }
        
        internal MarketAnswer addDiscountToProduct(string productID, DateTime startDate, DateTime endDate, int discountAmount, string discountType, bool presenteges)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
            Discount discount = new Discount(handler.getDiscountCode(), handler.GetdiscountTypeEnumString(discountType),
                startDate, endDate,discountAmount, presenteges);
            Stock.StockListItem SLI = stock.findstockListItembyProductID(productID);
            if (SLI == null) return new StoreAnswer(StoreEnum.ProductNotFound, "product " + productID + " does not exist in Stock");
            SLI.discount = discount;
            handler.dataLayer.addDiscount(discount);
            handler.dataLayer.EditStockInDatabase(SLI);
            return new StoreAnswer(StoreEnum.Success, "Discount added");
        }
        internal MarketAnswer removeDiscountFromProduct(string productID)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
            Stock.StockListItem SLI = stock.findstockListItembyProductID(productID);
            if (SLI == null) return new StoreAnswer(StoreEnum.ProductNotFound, "product " + productID + " does not exist in Stock");
            Discount discount = SLI.discount;
            handler.dataLayer.removeDiscount(discount);
            SLI.discount = null;
            handler.dataLayer.EditStockInDatabase(SLI);
            return new StoreAnswer(StoreEnum.Success, "discount remvoed");
        }
        internal void updateQuanityAfterPurches(Product product, int quantity)
        {
            Stock.StockListItem SLI = stock.findstockListItembyProductID(product.SystemId);
            if (SLI == null) throw new StoreException(-1, "Item not found");
            SLI.quantity = SLI.quantity - quantity;
            ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
            handler.dataLayer.EditStockInDatabase(SLI);
        }
        
        public LotteryTicket DoLottery(string product)
        {
            throw new NotImplementedException();
        }
        internal bool isOwner(User user) //waiting to Maor's function
        {
            return true;
        }
        public LinkedList<string> ViewPurchesHistory()
        {
            throw new NotImplementedException(); //waiting for Igor 4 help
        }
    }
}

