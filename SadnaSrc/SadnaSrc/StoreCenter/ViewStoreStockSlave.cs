using System;
using System.Collections.Generic;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class ViewStoreStockSlave
    {
        public MarketAnswer answer;
        private IUserShopper _shopper;
        IStoreDL storeLogic;

        public ViewStoreStockSlave(IUserShopper shopper, IStoreDL storeDL)
        {
            _shopper = shopper;
            storeLogic = storeDL;
        }

        public void ViewStoreStock(string storename)
        {
            try
            {
            MarketLog.Log("StoreCenter", "checking store stack");
            _shopper.ValidateCanBrowseMarket();
            MarketLog.Log("StoreCenter", "check if store exists");
            CheckIfStoreExists(storename);
            Store store = storeLogic.GetStorebyName(storename);
            LinkedList<string> result = new LinkedList<string>();
            LinkedList<string> IDS = storeLogic.GetAllStoreProductsID(store.SystemId);
            foreach (string item in IDS)
            {
                result.AddLast(GetProductStockInformation(item));
            }
            string[] resultArray = new string[result.Count];
            result.CopyTo(resultArray, 0);
            answer = new StoreAnswer(StoreEnum.Success, "", resultArray);
            }
            catch (StoreException e)
            {
                answer = new StoreAnswer(e);
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                answer = new StoreAnswer(StoreEnum.NoPremmision,
                    "User validation as valid customer has been failed . only valid users can browse market. Error message has been created!");
            }
        }

        private void CheckIfStoreExists(string storename)
        {
            if (!storeLogic.IsStoreExistAndActive(storename))
            {
                MarketLog.Log("StoreCenter", "store do not exists");
                throw new StoreException(StoreEnum.StoreNotExists, "store not exists or active");
            }
        }

        private string GetProductStockInformation(string productID)
        {
            StockListItem stockListItem = storeLogic.GetStockListItembyProductID(productID);
            if (stockListItem == null)
            {
                MarketLog.Log("storeCenter", "product not exists");
                throw new StoreException(StoreEnum.ProductNotFound, "product " + productID + " does not exist in Stock");
            }
            string discount = "";
            string product = stockListItem.Product.ToString();
            if (stockListItem.Discount != null)
                discount = stockListItem.Discount.ToString() + " , ";
            string purchaseWay = EnumStringConverter.PrintEnum(stockListItem.PurchaseWay);
            string quanitity = stockListItem.Quantity + "";
            string result = product + " , " + discount + purchaseWay + " , " + quanitity;
            return result;
        }
    }
}