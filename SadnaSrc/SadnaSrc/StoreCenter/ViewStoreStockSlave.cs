using System;
using System.Collections.Generic;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class ViewStoreStockSlave
    {
        internal MarketAnswer answer;
        private IUserShopper _shopper;
        ModuleGlobalHandler storeLogic;

        public ViewStoreStockSlave(IUserShopper shopper)
        {
            _shopper = shopper;
            storeLogic = ModuleGlobalHandler.GetInstance();
        }

        internal void ViewStoreStock(string storename)
        {
            MarketLog.Log("StoreCenter", "checking store stack");
            _shopper.ValidateCanBrowseMarket();
            MarketLog.Log("StoreCenter", "check if store exists");
            if (!storeLogic.DataLayer.IsStoreExistAndActive(storename))
            {
                MarketLog.Log("StoreCenter", "store do not exists");
                throw new StoreException(StoreEnum.StoreNotExists, "store not exists or active");
            }
            Store store = storeLogic.DataLayer.getStorebyName(storename);
            LinkedList<string> result = new LinkedList<string>();
            LinkedList<string> IDS = storeLogic.DataLayer.GetAllStoreProductsID(store.SystemId);
            foreach (string item in IDS)
            {
                result.AddLast(GetProductStockInformation(item));
            }
            string[] resultArray = new string[result.Count];
            result.CopyTo(resultArray, 0);
            answer = new StoreAnswer(StoreEnum.Success, "", resultArray);
        }
        private string GetProductStockInformation(string ProductID)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            StockListItem stockListItem = handler.DataLayer.GetStockListItembyProductID(ProductID);
            if (stockListItem == null)
            {
                MarketLog.Log("storeCenter", "product not exists");
                throw new StoreException(StoreEnum.ProductNotFound, "product " + ProductID + " does not exist in Stock");
            }
            string discount = "";
            string product = stockListItem.Product.ToString();
            if (stockListItem.Discount != null)
                discount = stockListItem.Discount.ToString() + " , ";
            string purchaseWay = handler.PrintEnum(stockListItem.PurchaseWay);
            string quanitity = stockListItem.Quantity + "";
            string result = product + " , " + discount + purchaseWay + " , " + quanitity;
            return result;
        }
    }
}