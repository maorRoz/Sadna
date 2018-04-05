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
        //private LinkedList<User> OtherOwners;
        //private LinkedList<User> Managers;
        private LinkedList<PurchasePolicy> PurchasePolicy;
        private LinkedList<LotterySaleManagmentTicket> lotterys;
        private LinkedList<String> history;
        private StoreService master { get; }
        private bool isActive { get; set; }

        public Store(User _Owner, string id, StoreService _master)
        {
            SystemId = id;
            stock = new Stock(SystemId);
           // OtherOwners = new LinkedList<User>();
          //  OtherOwners.AddFirst(_Owner);
           // Managers = new LinkedList<User>();
            PurchasePolicy = new LinkedList<PurchasePolicy>();
            isActive = true;
            master = _master;
            lotterys = new LinkedList<LotterySaleManagmentTicket>();
        }

        public bool IsStoreActive()
        {
            return isActive;
        }
        private StoreAnswer pAddDiscountToProduct(Product p, string _discountCode, discountTypeEnum _discountType, DateTime _startDate, DateTime _EndDate, int _DiscountAmount, bool _percentages)
        {
            if (_percentages&& _DiscountAmount >= 100)
            {
                return new StoreAnswer(StoreEnum.UpdateStockFail, "DiscountAmount is >= 100 and the discoint is presenteges");
            }
            if (_startDate < DateTime.Now.Date)
            {
                return new StoreAnswer(StoreEnum.UpdateStockFail, "can't set start time in the past");
            }
            if (_EndDate < DateTime.Now.Date)
            {
                return new StoreAnswer(StoreEnum.UpdateStockFail, "can't set end time to the past");
            }
            if (_startDate > _EndDate)
            {
                return new StoreAnswer(StoreEnum.UpdateStockFail, "can't set start time that is later then the discount end time");
            }
            Discount discount = new Discount(_discountCode, _discountType, _startDate, _EndDate, _DiscountAmount, _percentages);
            return stock.addDiscountToProduct(p, discount);
        }
        
        

        public MarketAnswer PromoteToOwner(User currentUser, User someoneToPromote)
        {
            if (IsOwner(currentUser)) {
                if (!OtherOwners.Contains(someoneToPromote))
                {
                    OtherOwners.AddLast(someoneToPromote);
                    return new StoreAnswer(StoreEnum.Success, "user " + someoneToPromote + " has been premoted to be a owner of store " + SystemId);
                }
                return new StoreAnswer(StoreEnum.AddStoreOwnerFail, "user " + someoneToPromote + " is Already a owner of the store " + SystemId);
            }
            return new StoreAnswer(StoreEnum.AddStoreOwnerFail, "user " + currentUser + " is not an owner of the store and can't make " + someoneToPromote + " to an owner");
        }


        public MarketAnswer PromoteToManager(User currentUser, User someoneToPromote)
        {    
               return new StoreAnswer(StoreEnum.Success, "user " + someoneToPromote + " has been premoted to be a owner of store " + SystemId);
        }


        public LinkedList<Product> getAllStoreProducts()
        {
            return stock.getAllProducts();
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
            return stock.addProductToStock(P, quantity);
        }

        public MarketAnswer removeProduct(string productID)
        {
            Product product = stock.getProductById(productID);
            if (product==null) { return new StoreAnswer(StoreEnum.ProductNotFound, "no Such Product"); }
            return stock.removeProductFromStock(product);
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
            stock.addAllProductsToExistingList(result);
        }
        private LotterySaleManagmentTicket getLotterySale(Product p)
        {
            foreach (LotterySaleManagmentTicket LSMT in lotterys)
            {
                if (LSMT.original.equal(p))
                    return LSMT;
            }
            return null;
        }
        public LotteryTicket MakeALotteryPurchase(string productID, int money)
        {
            LotteryTicket result = null;
            Product product = stock.getProductById(productID);
            if (product == null) return null;
            if (canPurchaseLottery(product, money))
            {
                result = getLotterySale(product).PurchaseALotteryTicket(money);

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
                stockListItem.PurchaseWay = purchaseEnum;
                result = new StoreAnswer(StoreEnum.Success, "item " + stockListItem.product.SystemId + " added PurchaseWay of" + newValue);
            }
            if (result == null) { return new StoreAnswer(StoreEnum.UpdateProductFail, "no leagal attrebute found"); }
            handler.dataLayer.EditStockInDatabase(stockListItem);
            return result;
        }
        internal LinkedList<Product> getAllProducts()
        {
            throw new NotImplementedException();
        }
        internal MarketAnswer getProductStockInformation(int productID)
        {
            throw new NotImplementedException();
        }
        internal MarketAnswer addDiscountToProduct(string productName, DateTime startDate, DateTime endDate, int discountAmount, string discountType, bool presenteges)
        {
            throw new NotImplementedException();
        }
        internal MarketAnswer removeDiscountFromProduct(string productID)
        {
            throw new NotImplementedException();
        }
        internal void updateQuanityAfterPurches(Product product, int quantity)
        {
            throw new NotImplementedException();
        }
        public LinkedList<string> ViewPurchesHistory()
        {
            throw new NotImplementedException(); //waiting for Igor 4 help
        }
        /public LotteryTicket DoLottery(string product)
        {
            Product p = stock.getProductById(product);
            LotteryTicket result = null;
            foreach (LotterySaleManagmentTicket LSMT in lotterys)
            {
                if (LSMT.original.equal(p))
                {
                    result = LSMT.Dolottery();
                }
            }
            return result;
        }
    }
}

