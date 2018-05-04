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
        public string _storeName;
        private IOrderSyncher syncher;
        private LinkedList<StockListItem> stockListItemToRemove;
        private LinkedList<Discount> discountsToRemvoe;
        private IStoreDL storeDL;
        public StoreManagementService(IUserSeller storeManager, string storeName)
        {
            _storeManager = storeManager;
            _storeName = storeName;
            global = StockSyncher.Instance;
            store = global.DataLayer.GetStorebyName(storeName);
            stockListItemToRemove = new LinkedList<StockListItem>();
            discountsToRemvoe = new LinkedList<Discount>();
            storeDL = StoreDL.GetInstance();
            syncher = new OrderSyncherHarmony();
        }

        public MarketAnswer CloseStore()
        {
            CloseStoreSlave slave = new CloseStoreSlave(_storeManager, _storeName, storeDL);
            slave.CloseStore();
            return slave.answer;
        }
        public MarketAnswer PromoteToStoreManager(string someoneToPromoteName, string actions)
        {
            PromoteToStoreManagerSlave slave = new PromoteToStoreManagerSlave(_storeManager, _storeName, storeDL);
            slave.PromoteToStoreManager(someoneToPromoteName, actions);
            return slave.Answer;
        }

        public MarketAnswer AddNewProduct(string _name, double _price, string _description, int quantity)
        {
            AddNewProductSlave slave = new AddNewProductSlave(_storeManager, _storeName, storeDL);
            StockListItem stockListItem = slave.AddNewProduct(_name, _price, _description, quantity);
            if (stockListItem != null)
                stockListItemToRemove.AddLast(stockListItem);
            return slave.answer;
        }

        public MarketAnswer AddNewLottery(string _name, double _price, string _description, DateTime startDate,
            DateTime endDate)
        {
            AddNewLotterySlave slave = new AddNewLotterySlave(_storeName, _storeManager, storeDL);
            StockListItem stockListItem = slave.AddNewLottery(_name, _price, _description, startDate, endDate);
            if (stockListItem != null)
                stockListItemToRemove.AddLast(stockListItem);
            return slave.answer;
        }

        public MarketAnswer RemoveProduct(string productName)
        {
            RemoveProductSlave slave = new RemoveProductSlave(syncher, _storeName, _storeManager, storeDL);
            slave.RemoveProduct(productName);
            return slave.Answer;
        }



        public MarketAnswer EditProduct(string productName, string whatToEdit, string newValue)
        {
            EditProductSlave slave = new EditProductSlave(_storeName, _storeManager, storeDL);
            slave.EditProduct(productName, whatToEdit, newValue);
            return slave.answer;
        }
        public void ClearSession()
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
            ChangeProductPurchaseWayToImmediateSlave slave = new ChangeProductPurchaseWayToImmediateSlave(_storeName, _storeManager, syncher, storeDL);
            slave.ChangeProductPurchaseWayToImmediate(productName);
            return slave.answer;
        }

        public MarketAnswer ChangeProductPurchaseWayToLottery(string productName, DateTime startDate, DateTime endDate)
        {
            ChangeProductPurchaseWayToLotterySlave slave = new ChangeProductPurchaseWayToLotterySlave(_storeName, _storeManager, storeDL);
            slave.ChangeProductPurchaseWayToLottery(productName, startDate, endDate);
            return slave.answer;
        }

        public MarketAnswer AddDiscountToProduct(string productName, DateTime startDate, DateTime endDate, int discountAmount, string discountType, bool presenteges)
        {
            AddDiscountToProductSlave slave = new AddDiscountToProductSlave(_storeName, _storeManager, storeDL);
            Discount discount = slave.AddDiscountToProduct(productName, startDate, endDate, discountAmount, discountType, presenteges);
            if (discount != null)
                discountsToRemvoe.AddLast(discount);
            return slave.answer;
        }
        public MarketAnswer EditDiscount(string productName, string whatToEdit, string newValue)
        {
            EditDiscountSlave slave = new EditDiscountSlave(_storeName, _storeManager, storeDL);
            slave.EditDiscount(productName, whatToEdit, newValue);
            return slave.answer;
        }



        public MarketAnswer RemoveDiscountFromProduct(string productName)
        {
            RemoveDiscountFromProductSlave slave = new RemoveDiscountFromProductSlave(_storeName, _storeManager, storeDL);
            slave.RemoveDiscountFromProduct(productName);
            return slave.Answer;
        }

        public MarketAnswer ViewStoreHistory()
        {
            ViewStoreHistorySlave slave = new ViewStoreHistorySlave(_storeName, _storeManager, storeDL);
            slave.ViewStoreHistory();
            return slave.answer;
        }


        public MarketAnswer AddQuanitityToProduct(string productName, int quantity)
        {
            AddQuanitityToProductSlave slave = new AddQuanitityToProductSlave(_storeName, _storeManager, storeDL);
            slave.AddQuanitityToProduct(productName, quantity);
            return slave.answer;
        }
    }
}