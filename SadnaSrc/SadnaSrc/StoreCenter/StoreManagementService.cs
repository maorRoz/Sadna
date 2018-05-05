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
        StockSyncher global;
        private IUserSeller _storeManager;
        public string StoreName;
        private IOrderSyncher syncher;
        private LinkedList<StockListItem> stockListItemToRemove;
        private LinkedList<Discount> discountsToRemvoe;
        private IStoreDL storeDL;
        public StoreManagementService(IUserSeller storeManager, string storeName)
        {
            _storeManager = storeManager;
            StoreName = storeName;
            global = StockSyncher.Instance;
            store = global.DataLayer.GetStorebyName(storeName);
            stockListItemToRemove = new LinkedList<StockListItem>();
            discountsToRemvoe = new LinkedList<Discount>();
            storeDL = StoreDL.Instance;
            syncher = new OrderSyncherHarmony();
        }

        public MarketAnswer CloseStore()
        {
            CloseStoreSlave slave = new CloseStoreSlave(_storeManager, StoreName, storeDL);
            slave.CloseStore();
            return slave.Answer;
        }
        public MarketAnswer PromoteToStoreManager(string someoneToPromoteName, string actions)
        {
            PromoteToStoreManagerSlave slave = new PromoteToStoreManagerSlave(_storeManager, StoreName, storeDL);
            slave.PromoteToStoreManager(someoneToPromoteName, actions);
            return slave.Answer;
        }

        public MarketAnswer AddNewProduct(string name, double price, string description, int quantity)
        {
            AddNewProductSlave slave = new AddNewProductSlave(_storeManager, StoreName, storeDL);
            StockListItem stockListItem = slave.AddNewProduct(name, price, description, quantity);
            if (stockListItem != null)
                stockListItemToRemove.AddLast(stockListItem);
            return slave.Answer;
        }

        public MarketAnswer AddNewLottery(string name, double price, string description, DateTime startDate,
            DateTime endDate)
        {
            AddNewLotterySlave slave = new AddNewLotterySlave(StoreName, _storeManager, storeDL);
            StockListItem stockListItem = slave.AddNewLottery(name, price, description, startDate, endDate);
            if (stockListItem != null)
                stockListItemToRemove.AddLast(stockListItem);
            return slave.Answer;
        }

        public MarketAnswer RemoveProduct(string productName)
        {
            RemoveProductSlave slave = new RemoveProductSlave(syncher, StoreName, _storeManager, storeDL);
            slave.RemoveProduct(productName);
            return slave.Answer;
        }

        public MarketAnswer EditProduct(string productName, string whatToEdit, string newValue)
        {
            EditProductSlave slave = new EditProductSlave(StoreName, _storeManager, storeDL);
            slave.EditProduct(productName, whatToEdit, newValue);
            return slave.Answer;
        }
        public MarketAnswer ChangeProductPurchaseWayToImmediate(string productName)
        {
            ChangeProductPurchaseWayToImmediateSlave slave = new ChangeProductPurchaseWayToImmediateSlave(StoreName, _storeManager, syncher, storeDL);
            slave.ChangeProductPurchaseWayToImmediate(productName);
            return slave.Answer;
        }

        public MarketAnswer ChangeProductPurchaseWayToLottery(string productName, DateTime startDate, DateTime endDate)
        {
            ChangeProductPurchaseWayToLotterySlave slave = new ChangeProductPurchaseWayToLotterySlave(StoreName, _storeManager, storeDL);
            slave.ChangeProductPurchaseWayToLottery(productName, startDate, endDate);
            return slave.Answer;
        }

        public MarketAnswer AddDiscountToProduct(string productName, DateTime startDate, DateTime endDate, int discountAmount, string discountType, bool presenteges)
        {
            AddDiscountToProductSlave slave = new AddDiscountToProductSlave(StoreName, _storeManager, storeDL);
            Discount discount = slave.AddDiscountToProduct(productName, startDate, endDate, discountAmount, discountType, presenteges);
            if (discount != null)
                discountsToRemvoe.AddLast(discount);
            return slave.Answer;
        }
        public MarketAnswer EditDiscount(string productName, string whatToEdit, string newValue)
        {
            EditDiscountSlave slave = new EditDiscountSlave(StoreName, _storeManager, storeDL);
            slave.EditDiscount(productName, whatToEdit, newValue);
            return slave.Answer;
        }

        public MarketAnswer RemoveDiscountFromProduct(string productName)
        {
            RemoveDiscountFromProductSlave slave = new RemoveDiscountFromProductSlave(StoreName, _storeManager, storeDL);
            slave.RemoveDiscountFromProduct(productName);
            return slave.Answer;
        }

        public MarketAnswer ViewStoreHistory()
        {
            ViewStoreHistorySlave slave = new ViewStoreHistorySlave(StoreName, _storeManager, storeDL);
            slave.ViewStoreHistory();
            return slave.Answer;
        }

        public MarketAnswer AddQuanitityToProduct(string productName, int quantity)
        {
            AddQuanitityToProductSlave slave = new AddQuanitityToProductSlave(StoreName, _storeManager, storeDL);
            slave.AddQuanitityToProduct(productName, quantity);
            return slave.Answer;
        }

        public MarketAnswer AddCategory(string categoryName)
        {
            AddCategorySlave slave = new AddCategorySlave(StoreName, _storeManager, storeDL);
            Category category =  slave.AddCategory(categoryName);
            return slave.Answer;
        }
        public MarketAnswer RemoveCategory(string categoryName)
        {
            RemoveCategorySlave slave = new RemoveCategorySlave(StoreName, _storeManager, storeDL);
            slave.RemoveCategory(categoryName);
            return slave.Answer;
        }
    }
}