using System;
using System.Collections.Generic;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class ViewStoreStockSlave
    {
        public MarketAnswer answer;
        private IUserShopper _shopper;
        IStoreDL storeLogic;
	    private string _storeName;

        public ViewStoreStockSlave(IUserShopper shopper, IStoreDL storeDL)
        {
            _shopper = shopper;
            storeLogic = storeDL;
			
        }

        public void ViewStoreStock(string storename)
        {
            try
            {
	         _storeName = storename;
			MarketLog.Log("StoreCenter", "checking store stack");
            _shopper.ValidateCanBrowseMarket();
            MarketLog.Log("StoreCenter", "check if store exists");
            CheckIfStoreExists(storename);
            Store store = storeLogic.GetStorebyName(storename);
            LinkedList<string> result = new LinkedList<string>();
            var IDS = storeLogic.GetAllStoreProductsID(store.SystemId);
            string info;
            foreach (string item in IDS)
            {
                info = GetProductStockInformation(item,false);
                if (info!="")
                    result.AddLast(info);
            }
            string[] resultArray = new string[result.Count];
            result.CopyTo(resultArray, 0);
            answer = new StoreAnswer(StoreEnum.Success, "", resultArray);
            }
            catch (StoreException e)
            {
                answer = new StoreAnswer((StoreEnum)e.Status,e.GetErrorMessage());
            }
            catch (DataException e)
            {
                answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                answer = new StoreAnswer(StoreEnum.NoPermission,
                    "User validation as valid customer has been failed . only valid users can browse market. Error message has been created!");
            }
        }

	    public void ViewStoreStockAll(string storename)
	    {
		    try
		    {
			    MarketLog.Log("StoreCenter", "checking store stack");
			    _shopper.ValidateCanBrowseMarket();
			    MarketLog.Log("StoreCenter", "check if store exists");
			    CheckIfStoreExists(storename);
			    Store store = storeLogic.GetStorebyName(storename);
			    LinkedList<string> result = new LinkedList<string>();
			    var IDS = storeLogic.GetAllStoreProductsID(store.SystemId);
			    string info;
			    foreach (string item in IDS)
			    {
				    info = GetProductStockInformation(item,true);
				    if (info != "")
					    result.AddLast(info);
			    }
			    string[] resultArray = new string[result.Count];
			    result.CopyTo(resultArray, 0);
			    answer = new StoreAnswer(StoreEnum.Success, "", resultArray);
		    }
		    catch (StoreException e)
		    {
				answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
			}
		    catch (DataException e)
		    {
		        answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
		    }
            catch (MarketException)
		    {
			    MarketLog.Log("StoreCenter", "no premission");
			    answer = new StoreAnswer(StoreEnum.NoPermission,
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

        private string GetProductStockInformation(string productID, bool showAll)
        {
            StockListItem stockListItem = storeLogic.GetStockListItembyProductID(productID);
	        
			if (stockListItem == null)
            {  
				MarketLog.Log("storeCenter", "product not exists");
                throw new StoreException(StoreEnum.ProductNotFound, "product " + productID + " does not exist in Stock");
            }
            if (stockListItem.PurchaseWay == PurchaseEnum.Lottery && !showAll)
            {
                LotterySaleManagmentTicket managmentTicket =
                    storeLogic.GetLotteryByProductID((productID));
                StockListItem sli = storeLogic.GetStockListItembyProductID(productID);
                if ((managmentTicket.EndDate < MarketYard.MarketDate) ||
                    (managmentTicket.StartDate > MarketYard.MarketDate) ||
                    ((managmentTicket.TotalMoneyPayed == managmentTicket.ProductNormalPrice)&& sli.Quantity==0))
                    return "";
            }
	        Discount  totalDiscount = stockListItem.CalcTotalDiscount(_storeName);
			string discountString = " Discount: {";
            string product = stockListItem.Product.ToString();
            if (totalDiscount != null)
	            discountString += totalDiscount;
            else
            {
	            discountString += "null";
            }
	        discountString += "}";
            string purchaseWay = " Purchase Way: " + EnumStringConverter.PrintEnum(stockListItem.PurchaseWay);
            string quanitity = " Quantity: "+stockListItem.Quantity ;
            string result = product + discountString + purchaseWay + quanitity;
            return result;
        }
    }
}