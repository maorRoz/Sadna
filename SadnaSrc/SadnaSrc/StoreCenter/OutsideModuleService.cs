﻿using SadnaSrc.UserSpot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    interface OutsideModuleService
    {
        LinkedList<Store> GetAllUserStores(User user);
        LinkedList<Store> GetAllStores();
        Store GetStoreByID(int ID); // this one if you need extra help 
        Store GetStoreByID(string ID);
        LinkedList<Product> GetAllMarketProducts(); // you will need it. I don't know how to return it by MarketAnswer
        void UpdateQuantityAfterPurches(string storeID, string productID, int quantity);
    }
}
