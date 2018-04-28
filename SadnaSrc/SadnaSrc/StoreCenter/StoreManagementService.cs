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
        private IOrderSyncher syncher;
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
            syncher = new OrderSyncherHarmony();
        }

        public MarketAnswer CloseStore()
        {
            CloseStoreSlave slave = new CloseStoreSlave(_storeManager, ref store);
            slave.closeStore();
            return slave.answer;
        }
        public MarketAnswer PromoteToStoreManager(string someoneToPromoteName, string actions)
        {
            PromoteToStoreManagerSlave slave = new PromoteToStoreManagerSlave(_storeManager, _storeName);
            slave.PromoteToStoreManager(someoneToPromoteName, actions);
            return slave.answer;
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
            AddNewProductSlave slave = new AddNewProductSlave(_storeManager, _storeName);
            StockListItem stockListItem = slave.AddNewProduct(_name, _price, _description, quantity);
            if (stockListItem!=null)
                stockListItemToRemove.AddLast(stockListItem);
            return slave.answer;
        }

        public MarketAnswer AddNewLottery(string _name, double _price, string _description, DateTime startDate,
            DateTime endDate)
        {
            AddNewLotterySlave slave = new AddNewLotterySlave(_storeName,_storeManager);
            StockListItem stockListItem =  slave.AddNewLottery(_name, _price, _description, startDate, endDate);
            if (stockListItem!=null)
                stockListItemToRemove.AddLast(stockListItem);
            return slave.answer;
        }

        public MarketAnswer RemoveProduct(string productName)
        {
            RemoveProductSlave slave = new RemoveProductSlave(ref syncher, _storeName, _storeManager);
            slave.RemoveProduct(productName);
            return slave.answer;
        }



        public MarketAnswer EditProduct(string productName, string whatToEdit, string newValue)
        {
            EditProductSlave slave = new EditProductSlave(_storeName,_storeManager);
            slave.EditProduct(productName, whatToEdit, newValue);
            return slave.answer;
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
            ChangeProductPurchaseWayToImmediateSlave slave = new ChangeProductPurchaseWayToImmediateSlave(_storeName, _storeManager,syncher);
            slave.ChangeProductPurchaseWayToImmediate(productName);
            return slave.answer;
        }

        public MarketAnswer ChangeProductPurchaseWayToLottery(string productName, DateTime startDate, DateTime endDate)
        {
            ChangeProductPurchaseWayToLotterySlave slave = new ChangeProductPurchaseWayToLotterySlave(_storeName, _storeManager);
            slave.ChangeProductPurchaseWayToLottery(productName, startDate, endDate);
            return slave.answer;
        }


        public MarketAnswer AddDiscountToProduct(string productName, DateTime startDate, DateTime endDate, int discountAmount, string discountType, bool presenteges)
        {
            AddDiscountToProductSlave slave = new AddDiscountToProductSlave(_storeName, _storeManager);
            Discount discount = slave.AddDiscountToProduct(productName, startDate, endDate, discountAmount, discountType, presenteges);
            if (discount!=null)
                discountsToRemvoe.AddLast(discount);
            return slave.answer;
        }
        public MarketAnswer EditDiscount(string productName, string whatToEdit, string newValue)
        {
            EditDiscountSlave slave = new EditDiscountSlave(_storeName, _storeManager);
            slave.EditDiscount(productName, whatToEdit, newValue);
            return slave.answer;
        }



        public MarketAnswer RemoveDiscountFromProduct(string productName)
        {
            RemoveDiscountFromProductSlave slave = new RemoveDiscountFromProductSlave(_storeName, _storeManager);
            slave.RemoveDiscountFromProduct(productName);
            return slave.answer;
        }
           
        public MarketAnswer ViewStoreHistory()
        {
            ViewStoreHistorySlave slave = new ViewStoreHistorySlave(store, _storeManager);
            slave.ViewStoreHistory();
            return slave.answer;
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

            syncher.CleanSession();
        }

        public MarketAnswer AddQuanitityToProduct(string productName, int quantity)
        {
            AddQuanitityToProductSlave slave = new AddQuanitityToProductSlave(_storeName, _storeManager);
            slave.AddQuanitityToProduct(productName, quantity);
            return slave.answer;
        }
    }
}