﻿using System;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class ViewStoreInfoSlave
    {
        public MarketAnswer answer;
        private IUserShopper _shopper;
        IStoreDL storeLogic;

        public ViewStoreInfoSlave(IUserShopper shopper, IStoreDL storeDL)
        {
            _shopper = shopper;
            storeLogic = storeDL;
        }

        public void ViewStoreInfo(string store)
        {
            try
            {
                CheckIfStoreExistsAndActive(store);
                MarketLog.Log("StoreCenter", "check that have premission to view store info");
                _shopper.ValidateCanBrowseMarket();
                MarketLog.Log("StoreCenter", "premission gained");
                string[] storeInfo = storeLogic.GetStoreInfo(store);
                CheckIfStoreInfoIsNotNull(storeInfo);
                MarketLog.Log("StoreCenter", "info gained");
                answer = new StoreAnswer(ViewStoreStatus.Success, "Store info has been successfully granted!", storeInfo);
            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "");
                answer = new StoreAnswer((ViewStoreStatus)e.Status, "Something is wrong with viewing " + store +
                                                                  " info by customers . Error message has been created!");
            }
            catch (DataException e)
            {
                answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                answer = new StoreAnswer(ViewStoreStatus.InvalidUser,
                    "User validation as valid customer has been failed . only valid users can browse market. Error message has been created!");
            }
        }
        private void CheckIfStoreInfoIsNotNull(object item)
        {
            if (item == null)
            { throw new StoreException(ViewStoreStatus.NoStore, "No store found"); }
        }
        private void CheckIfStoreExistsAndActive(string _storename)
        {
            if (!storeLogic.IsStoreExistAndActive(_storename))
            { throw new StoreException(ViewStoreStatus.NoStore, "store not exists or active"); }
        }
    }
}