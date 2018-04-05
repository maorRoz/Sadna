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
        private LinkedList<User> OtherOwners;
        private LinkedList<User> Managers;
        private LinkedList<PurchasePolicy> PurchasePolicy;
        private LinkedList<LotterySaleManagmentTicket> lotterys;
        private LinkedList<String> history;
        private StoreService master { get; }
        private bool isActive { get; set; }

        public Store(User _Owner, string id, StoreService _master)
        {
            SystemId = id;
            stock = new Stock(SystemId);
            OtherOwners = new LinkedList<User>();
            OtherOwners.AddFirst(_Owner);
            Managers = new LinkedList<User>();
            PurchasePolicy = new LinkedList<PurchasePolicy>();
            isActive = true;
            master = _master;
            lotterys = new LinkedList<LotterySaleManagmentTicket>();
        }
        public bool IsOwner(User user)
        {
            if (isActive)
            {
                bool result = false;
                foreach (User other in OtherOwners)
                {
                    result = result || (user.SystemID == other.SystemID);
                }
                return result;
            }
            return false;
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
        private StoreAnswer pRemoveDiscountToProduct(string productID)
        {
            Product p = stock.getProductById(productID);
            return stock.removeDiscountToProduct(p);
        }

        private StoreAnswer pChangeProductPurchaseWay(Product p, PurchaseEnum Purchase, DateTime startDate, DateTime endDate)
        {
            if (Purchase == PurchaseEnum.LOTTERY)
            {
                if (getLotterySale(p) == null) {
                    if (LotterySaleManagmentTicket.checkDates(startDate, endDate)) {
                        lotterys.AddLast(new LotterySaleManagmentTicket(master.getLottyerID(), p, startDate, endDate));
                        return stock.addPurchaseWayToProduct(p, Purchase);
                    }
                    return new StoreAnswer(StoreEnum.ChangePurchaseTypeFail, "the dates are wrong or in the past");
                }
                return new StoreAnswer(StoreEnum.ChangePurchaseTypeFail, "product " + p.SystemId + " is already in lottery mode");
            }
            return stock.addPurchaseWayToProduct(p, Purchase);
        }
        
        internal LinkedList<Product> getAllProducts()
        {
            throw new NotImplementedException();
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

        internal MarketAnswer getProductStockInformation(int productID)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer PromoteToManager(User currentUser, User someoneToPromote)
        {
            if (IsOwner(currentUser))
            {
                if (!Managers.Contains(someoneToPromote))
                {
                    Managers.AddLast(someoneToPromote);
                    return new StoreAnswer(StoreEnum.Success, "user " + someoneToPromote + " has been premoted to be a owner of store " + SystemId);
                }
                return new StoreAnswer(StoreEnum.AddStoreManagerFail, "user " + someoneToPromote + " is Already a manager of the store " + SystemId);
            }
            return new StoreAnswer(StoreEnum.AddStoreOwnerFail, "user " + currentUser + " is not an owner of the store and can't make " + someoneToPromote + " to a manager");
        }


        public LinkedList<Product> getAllStoreProducts()
        {
            return stock.getAllProducts();
        }

        internal MarketAnswer addDiscountToProduct(string productName, DateTime startDate, DateTime endDate, int discountAmount, string discountType, bool presenteges)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer CloseStore(User ownerOrSystemAdmin)
        {
            if (IsOwner(ownerOrSystemAdmin)) {
                if (isActive)
                {
                    isActive = false;
                    return new StoreAnswer(StoreEnum.Success, "store " + SystemId + " closed");
                }
                return new StoreAnswer(StoreEnum.Success, "store " + SystemId + " is alrady closed");
            }
            return new StoreAnswer(StoreEnum.CloseStoreFail, "user " + ownerOrSystemAdmin + " is not a System admin and not an owner of the store " + SystemId);
        }

        internal MarketAnswer removeDiscountFromProduct(string productID)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer AddProduct(string _name, int _price, string _description, int quantity)
        {
            Product P = new Product(master.getProductID(), _name, _price, _description);
            return stock.addProductToStock(P, quantity);
        }

        public MarketAnswer IncreaseProductQuantity(string productID, int quantity)
        {
            Product product = stock.getProductById(productID);
            return stock.addProductToStock(product, quantity);
        }

        public MarketAnswer removeProduct(string productID)
        {
            Product product = stock.getProductById(productID);
            return stock.removeProductFromStock(product);
        }
        public MarketAnswer ChangeProductPurchaseWayToImmediate(string productID)
        {
            Product product = stock.getProductById(productID);
            return pChangeProductPurchaseWay(product, PurchaseEnum.IMMEDIATE, DateTime.Now, DateTime.Now);
        }

        internal double getProductPriceWithDiscountbyDouble(string productName, int discountCode, int quantity)
        {
            throw new NotImplementedException();
        }

        public MarketAnswer ChangeProductPurchaseWayToLottery(string productID, DateTime StartDate, DateTime EndDate)
        {
            Product product = stock.getProductById(productID);
            return pChangeProductPurchaseWay(product, PurchaseEnum.LOTTERY, StartDate, EndDate);
        }
        public MarketAnswer addDiscountToProduct_VISIBLE(string productID, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            Product product = stock.getProductById(productID);
            return pAddDiscountToProduct(product, master.getDiscountCode(), discountTypeEnum.VISIBLE, _startDate, _EndDate, _DiscountAmount, false);
        }

        /**public MarketAnswer addDiscountToProduct_HIDDEN(string productID, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            Product product = stock.getProductById(productID);
            return pAddDiscountToProduct(product, master.getDiscountCode(), discountTypeEnum.HIDDEN, _startDate, _EndDate, _DiscountAmount, false);
        }

        public MarketAnswer addDiscountToProduct_presenteges_VISIBLE(string productID, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            Product product = stock.getProductById(productID);
            if (_DiscountAmount < 100)
            {
                return pAddDiscountToProduct(product, master.getDiscountCode(), discountTypeEnum.VISIBLE, _startDate, _EndDate, _DiscountAmount, true);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "DiscountAmount is >= 100");
        }

        public MarketAnswer addDiscountToProduct_presenteges_HIDDEN(string productID, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            if (_DiscountAmount < 100)
            {
                Product product = stock.getProductById(productID);
                return pAddDiscountToProduct(product, master.getDiscountCode(), discountTypeEnum.HIDDEN, _startDate, _EndDate, _DiscountAmount, true);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "DiscountAmount is >= 100");
        }

        public MarketAnswer removeDiscountFormProduct(string productID)
        {
            Product product = stock.getProductById(productID);
            return stock.removeDiscountToProduct(product);
        }

        public MarketAnswer EditDiscountToPrecenteges(string productID)
        {
            Product product = stock.getProductById(productID);
            return stock.EditDiscountPrecenteges(product, true);
        }

        public MarketAnswer EditDiscountToNonPrecenteges(string productID)
        {
            Product product = stock.getProductById(productID);
            return stock.EditDiscountPrecenteges(product, false);
        }

        public MarketAnswer EditDiscountToHidden(string productID)
        {
            Product product = stock.getProductById(productID);
            return stock.EditDiscountMode(product, discountTypeEnum.HIDDEN);
        }

        public MarketAnswer EditDiscountToVisible(string productID)
        {
            Product product = stock.getProductById(productID);
            return stock.EditDiscountMode(product, discountTypeEnum.VISIBLE);
        }

        public MarketAnswer EditDiscountAmount(String productID, int amount)
        {
            Product product = stock.getProductById(productID);
            return stock.EditDiscountAmount(product, amount);
        }

        public MarketAnswer EditDiscountStartTime(String productID, DateTime _startDate)
        {
            Product product = stock.getProductById(productID);
            return stock.EditDiscountStartTime(product, _startDate);
        }

        public MarketAnswer EditDiscountEndTime(string productID, DateTime _EndDate)
        {
            Product product = stock.getProductById(productID);
            return stock.EditDiscountEndTime(product, _EndDate);
        }**/

        public Product getProductById(string ID) //will return null if product is not exists
        {
            return stock.getProductById(ID);
        }
        public Discount getProductDiscountByProductID(string ID)//will return null if product is not exists or discount not exists
        {
            Product temp = stock.getProductById(ID);
            return stock.getProductDiscount(temp);
        }

        public PurchaseEnum getProductPurchaseWayByProductID(string ID)//will return PRODUCTNOTFOUND if product is not exists
        {
            Product temp = stock.getProductById(ID);
            return stock.getProductPurchaseWay(temp);
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
            if (canPurchaseLottery(productID, money))
            {
                result = getLotterySale(product).PurchaseALotteryTicket(money);

            }
            return result;
        }

        public Product MakeAImmediatePurchase(string productID, int quantity)
        {
            Product product = stock.getProductById(productID);
            if (canPurchaseImmediate(productID, quantity))
            {
                stock.UpdateQuantityAfterPruchese(product, quantity);
                return product;
            }
            return null;
        }
     
        public MarketAnswer getProductPriceWithDiscount(string _product, int _DiscountCode, int _quantity)
        {
            double result = stock.CalculateSingleItemPrice(stock.getProductById(_product), _DiscountCode, _quantity);
            return new StoreAnswer(StoreEnum.Success, "" + result);
        }
        public LotteryTicket DoLottery(string product)
        {
            Product p = stock.getProductById(product);
            LotteryTicket result = null;
            foreach (LotterySaleManagmentTicket LSMT in lotterys)
            {
                if (LSMT.original.equal(p))
                {
                    result= LSMT.Dolottery();
                }
            }
            return result;
        }

        public bool canPurchaseImmediate(Product product, int quantity)
        {
            Stock.StockListItem SLI = stock.findstockListItembyProductID(product.SystemId);
            return (SLI.quantity >= quantity);
        }

        public bool canPurchaseLottery(Product product, int amountOfMoney)
        {
            Stock.StockListItem SLI = stock.findstockListItembyProductID(product.SystemId);
            return ((SLI.PurchaseWay == PurchaseEnum.LOTTERY) &&
                    (SLI.quantity > 0) &&
                    (getLotterySale(product) != null) &&
                    (getLotterySale(product).CanPurchase(amountOfMoney)));
        }

        public LinkedList<string> ViewPurchesHistory()
        {
            throw new NotImplementedException(); //waiting for Igor 4 help
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
    }
}

