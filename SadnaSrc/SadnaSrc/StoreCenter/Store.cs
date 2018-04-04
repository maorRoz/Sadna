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
    public class Store : IStore
    {
        public String SystemId { get; }
        private Stock stock { get; set; }
        private int Owner;
        private LinkedList<int> OtherOwners;
        private LinkedList<int> Managers;
        private LinkedList<PurchasePolicy> PurchasePolicy;
        private LinkedList<LotterySaleManagmentTicket> lotterys;
        private LinkedList<String> history;
        private StoreService master { get; }
        private bool isActive { get; set; }

        public Store(int _Owner, int id, StoreService _master)
        {
            SystemId = "S"+id;
            stock = new Stock();
            Owner = _Owner;
            OtherOwners = new LinkedList<int>();
            OtherOwners.AddFirst(_Owner);
            Managers = new LinkedList<int>();
            PurchasePolicy = new LinkedList<PurchasePolicy>();
            isActive = true;
            master = _master;
            lotterys = new LinkedList<LotterySaleManagmentTicket>();
        }
        private StoreAnswer paddProduct(Product product, int quantity)
        {
            return stock.addProductToStock(product, quantity);
        }
        public bool IsOwner(int user)
        {
            if (isActive)
            {
                bool result = (user == Owner);
                if (result) { return result; }
                foreach (int other in OtherOwners)
                {
                    result = result || (user == other);
                }
                return result;
            }
            return false;
        }
        public bool IsStoreActive()
        {
            return isActive;
        }
        private StoreAnswer pAddDiscountToProduct(Product p, int _discountCode, discountTypeEnum _discountType, DateTime _startDate, DateTime _EndDate, int _DiscountAmount, bool _presenteges)
        {
            if (_presenteges && _DiscountAmount >= 100)
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
            Discount discount = new Discount(_discountCode, _discountType, _startDate, _EndDate, _DiscountAmount, _presenteges);
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

        public MarketAnswer PromoteToOwner(int currentUser, int someoneToPromote)
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

        public MarketAnswer PromoteToManager(int currentUser, int someoneToPromote)
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



        public MarketAnswer CloseStore(int ownerOrSystemAdmin)
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

        public MarketAnswer editProductPrice(string productID, int newprice)
        {
            Product product = stock.getProductById(productID);
            return stock.editProductPrice(product, newprice);
        }

        public MarketAnswer editProductName(string productID, string Name)
        {
            Product product = stock.getProductById(productID);
            return stock.editProductName(product, Name);
        }

        public MarketAnswer editProductDescripiton(string productID, string Desccription)
        {
            Product product = stock.getProductById(productID);
            return stock.editProductDescripiton(product, Desccription);
        }

        public MarketAnswer ChangeProductPurchaseWayToImmediate(string productID)
        {
            Product product = stock.getProductById(productID);
            return pChangeProductPurchaseWay(product, PurchaseEnum.IMMEDIATE, DateTime.Now, DateTime.Now);
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

        public MarketAnswer addDiscountToProduct_HIDDEN(string productID, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
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
        }

        public string getProductById(string ID) //will return null if product is not exists
        {
            return stock.getProductById(ID).ToString();
        }
        public string getProductDiscountByProductID(string ID)//will return null if product is not exists or discount not exists
        {
            Product temp = stock.getProductById(ID);
            return stock.getProductDiscount(temp).toString();
        }

        public string getProductPurchaseWayByProductID(string ID)//will return PRODUCTNOTFOUND if product is not exists
        {
            Product temp = stock.getProductById(ID);
            return printEnum(stock.getProductPurchaseWay(temp));
        }

        public int getProductQuantitybyProductID(string ID)//will return -1 if product is not exists
        {
            Product temp = stock.getProductById(ID);
            return stock.Quantity(temp);
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
        public string MakeALotteryPurchase(string productID, int moeny)
        {
            LotteryTicket result = null;
            Product product = stock.getProductById(productID);
            if (canPurchaseLottery(productID, moeny))
            {
                result = getLotterySale(product).PurchaseALotteryTicket(moeny);
                history.AddLast("a user Purchased a Lottery ticket of the  Product " + product.SystemId);

            }
            return result.toString();
        }

        public string MakeAImmediatePurchase(string productID, int quantity)
        {
            Product product = stock.getProductById(productID);
            if (canPurchaseImmediate(productID, quantity))
            {
                stock.UpdateQuantityAfterPruchese(product, quantity);
                history.AddLast( "a user Purchased by ImmidatePurchase the Product " + productID);
                return product.toString();
            }
            return "";
        }
     
        public double getProductPrice(string _product, int _DiscountCode, int _quantity)
        {
            return stock.CalculateSingleItemPrice(stock.getProductById(_product), _DiscountCode, _quantity);
        }
        public string DoLottery(string product)
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
            return result.toString();
        }
        public LinkedList<string> ViewPurchaseHistory()
        {
            return history;
        }

        public bool canPurchaseImmediate(string product, int quantity)
        {
            return (stock.Quantity(stock.getProductById(product)) >= quantity);
        }

        public bool canPurchaseLottery(string product, int amountOfMoney)
        {
            Product p = stock.getProductById(product);
            return ((stock.getProductPurchaseWay(p) == PurchaseEnum.LOTTERY) &&
                    (stock.Quantity(p) > 0) &&
                    (getLotterySale(p) != null) &&
                    (getLotterySale(p).CanPurchase(amountOfMoney)));
        }
        public string ToString()
        {
            return "storeId: " + SystemId;
        }
        private string printEnum(PurchaseEnum purchaseEnum)
        {
            if (purchaseEnum == PurchaseEnum.IMMEDIATE) return "IMMIDIATE";
            if (purchaseEnum == PurchaseEnum.LOTTERY) return "LOTTERY";
            return "PRODUCTNOTFOUND";
        }

    }
}

