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
        public int SystemId { get; }
        private Stock stock { get; set; }
        private User Owner;
        private LinkedList<User> OtherOwners;
        private LinkedList<User> Managers;
        private LinkedList<PurchesPolicy> purchesPolicy;
        private LinkedList<LotterySaleManagmentTicket> lotterys;
        private StoreService master { get; }
        private bool isActive { get; set; }

        public Store(User _Owner, int id, StoreService _master)
        {
            SystemId = id;
            stock = new Stock();
            Owner = _Owner;
            OtherOwners = new LinkedList<User>();
            OtherOwners.AddFirst(_Owner);
            Managers = new LinkedList<User>();
            purchesPolicy = new LinkedList<PurchesPolicy>();
            isActive = true;
            master = _master;
            lotterys = new LinkedList<LotterySaleManagmentTicket>();
        }
        private StoreAnswer paddProduct(Product product, int quantity)
        {
            return stock.addProductToStock(product, quantity);
        }
        public bool IsOwner(User user)
        {
            if (isActive)
            {
                bool result = (user.SystemID == Owner.SystemID);
                if (result) { return result; }
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
        private StoreAnswer pAddDiscountToProduct(Product p, int _discountCode, discountTypeEnum _discountType, DateTime _startDate, DateTime _EndDate, int _DiscountAmount, bool _presenteges)
        {
            if (_presenteges&& _DiscountAmount>=100)
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
        private StoreAnswer pRemoveDiscountToProduct(Product p)
        {
            return stock.removeDiscountToProduct(p);
        }

        private StoreAnswer pChangeProductPurchesWay(Product p, PurchesEnum purches, DateTime startDate, DateTime endDate)
        {
            if (purches == PurchesEnum.LOTTERY)
            {
                if (getLotterySale(p)==null) {
                    if (LotterySaleManagmentTicket.checkDates(startDate, endDate)) {
                        lotterys.AddLast(new LotterySaleManagmentTicket(master.getLottyerID(), p, startDate, endDate));
                      return stock.addPurchesWayToProduct(p, purches);
                    }
                    return new StoreAnswer(StoreEnum.ChangePurchesTypeFail, "the dates are wrong or in the past");
                }
                return new StoreAnswer(StoreEnum.ChangePurchesTypeFail, "product "+p.SystemId+" is already in lottery mode");
            }
            return stock.addPurchesWayToProduct(p, purches);
        }

        public MarketAnswer PromoteToOwner(User currentUser, User someoneToPromote)
        {
            if (IsOwner(currentUser)) { 
               if (!OtherOwners.Contains(someoneToPromote))
                {
                    OtherOwners.AddLast(someoneToPromote);
                    return new StoreAnswer(StoreEnum.Success, "user " + someoneToPromote.SystemID + " has been premoted to be a owner of store " + SystemId);
                }
                return new StoreAnswer(StoreEnum.AddStoreOwnerFail, "user " + someoneToPromote.SystemID + " is Already a owner of the store " + SystemId);
            }
            return new StoreAnswer(StoreEnum.AddStoreOwnerFail, "user " + currentUser.SystemID + " is not an owner of the store and can't make " + someoneToPromote.SystemID + " to an owner");
        }

        public MarketAnswer PromoteToManager(User currentUser, User someoneToPromote)
        {
            if (IsOwner(currentUser))
            {
                if (!Managers.Contains(someoneToPromote))
            {
                Managers.AddLast(someoneToPromote);
                return new StoreAnswer(StoreEnum.Success, "user " + someoneToPromote.SystemID + " has been premoted to be a owner of store " + SystemId);
            }
            return new StoreAnswer(StoreEnum.AddStoreManagerFail, "user " + someoneToPromote.SystemID + " is Already a manager of the store " + SystemId);
        }
        return new StoreAnswer(StoreEnum.AddStoreOwnerFail, "user " + currentUser.SystemID + " is not an owner of the store and can't make " + someoneToPromote.SystemID + " to a manager");
    }
   
        public LinkedList<Product> getAllStoreProducts()
        {
            return stock.getAllProducts();
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
            return new StoreAnswer(StoreEnum.CloseStoreFail, "user " + ownerOrSystemAdmin.SystemID + " is not a System admin and not an owner of the store " + SystemId);
        }
        
        public MarketAnswer AddProduct(string _name, int _price, string _description, int quantity)
        {
            Product P = new Product(master.getProductID(), _name, _price, _description);
            return stock.addProductToStock(P, quantity);
        }

        public MarketAnswer IncreaseProductQuantity(Product product, int quantity)
        {
            return stock.addProductToStock(product, quantity);
        }

        public MarketAnswer removeProduct(Product product)
        {
            return stock.removeProductFromStock(product);
        }

        public MarketAnswer editProductPrice(Product product, int newprice)
        {
            return stock.editProductPrice(product, newprice);
        }

        public MarketAnswer editProductName(Product product, string Name)
        {
            return stock.editProductName(product, Name);
        }

        public MarketAnswer editProductDescripiton(Product product, string Desccription)
        {
            return stock.editProductDescripiton(product, Desccription);
        }

        public MarketAnswer ChangeProductPurchesWayToImmidiate(Product product)
        {
            return pChangeProductPurchesWay(product, PurchesEnum.IMMIDIATE, DateTime.Now, DateTime.Now);
        }

        public MarketAnswer ChangeProductPurchesWayToLottery(Product product, DateTime StartDate, DateTime EndDate)
        {
            return pChangeProductPurchesWay(product, PurchesEnum.LOTTERY, StartDate, EndDate);
        }

        public MarketAnswer addDiscountToProduct_VISIBLE(Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            return pAddDiscountToProduct(product, master.getDiscountCode(), discountTypeEnum.VISIBLE, _startDate, _EndDate, _DiscountAmount, false);
        }

        public MarketAnswer addDiscountToProduct_HIDDEN(Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            return pAddDiscountToProduct(product, master.getDiscountCode(), discountTypeEnum.HIDDEN, _startDate, _EndDate, _DiscountAmount, false);
        }

        public MarketAnswer addDiscountToProduct_presenteges_VISIBLE(Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            if (_DiscountAmount < 100)
            {
                return pAddDiscountToProduct(product, master.getDiscountCode(), discountTypeEnum.VISIBLE, _startDate, _EndDate, _DiscountAmount, true);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "DiscountAmount is >= 100");
        }

        public MarketAnswer addDiscountToProduct_presenteges_HIDDEN(Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount)
        {
            if (_DiscountAmount < 100)
            {
                return pAddDiscountToProduct(product, master.getDiscountCode(), discountTypeEnum.HIDDEN, _startDate, _EndDate, _DiscountAmount, true);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "DiscountAmount is >= 100");
    }

        public MarketAnswer removeDiscountFormProduct(Product product)
        {
            return stock.removeDiscountToProduct(product);
        }

        public MarketAnswer EditDiscountToPrecenteges(Product product)
        {
            return stock.EditDiscountPrecenteges(product, true);
        }

        public MarketAnswer EditDiscountToNonPrecenteges(Product product)
        {
            return stock.EditDiscountPrecenteges(product, false);
        }

        public MarketAnswer EditDiscountToHidden(Product product)
        {
            return stock.EditDiscountMode(product, discountTypeEnum.HIDDEN);
        }
        
        public MarketAnswer EditDiscountToVisible(Product product)
        {
            return stock.EditDiscountMode(product, discountTypeEnum.VISIBLE);
        }

        public MarketAnswer EditDiscountAmount(Product product, int amount)
        {
            return stock.EditDiscountAmount(product, amount);
        }

        public MarketAnswer EditDiscountStartTime(Product product, DateTime _startDate)
        {
            return stock.EditDiscountStartTime(product, _startDate);
        }

        public MarketAnswer EditDiscountEndTime(Product product, DateTime _EndDate)
        {
            return stock.EditDiscountEndTime(product, _EndDate);
        }

       
        public Product getProductById(int ID) //will return null if product is not exists
        {
            return stock.getProductById(ID);
        }
        public Discount getProductDiscountByProductID(int ID)//will return null if product is not exists or discount not exists
        {
            Product temp = stock.getProductById(ID);
            return stock.getProductDiscount(temp);
        }

        public PurchesEnum getProductPurchesWayByProductID(int ID)//will return PRODUCTNOTFOUND if product is not exists
        {
            Product temp = stock.getProductById(ID);
            return stock.getProductPurchaseWay(temp);
        }
        public int getProductQuantitybyProductID(int ID)//will return -1 if product is not exists
        {
            Product temp = stock.getProductById(ID);
            return stock.getProductQuantity(temp);
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
        public LotteryTicket MakeALotteryPurches(Product product, int moeny)
        {
            throw new NotImplementedException();
        }

        public Product MakeAImmidiatePurches(Product product)
        {
            throw new NotImplementedException();
        }

        public LinkedList<string> ViewPurchesHistory()
        {
            throw new NotImplementedException();
        }

        public bool canPurchesImmidiate(Product product, int quantity)
        {
            return (stock.getProductQuantity(product) >= quantity);
        }

        public bool canPurchesLottery(Product product, int amountOfMoney)
        {
            return ((stock.getProductPurchaseWay(product) == PurchesEnum.LOTTERY) &&
                    (stock.getProductQuantity(product) > 0) &&
                    (getLotterySale(product) != null) &&
                    (getLotterySale(product).CanPurches(amountOfMoney)));
        }
    }
}

