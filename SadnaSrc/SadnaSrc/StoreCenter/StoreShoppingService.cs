﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class StoreShoppingService : IStoreShoppingService
    {
        private readonly IUserShopper _shopper;
        private readonly LinkedList<Store> stores;
        private readonly IStoreDL storeDL;
        public StoreShoppingService(IUserShopper shopper)
        {
            _shopper = shopper;
            stores = new LinkedList<Store>();
            storeDL = StoreDL.Instance;
        }
        public void LoginShoper(string userName, string password)
        {
            ((UserShopperHarmony)_shopper).LogInShopper(userName, password);
        }
        public void MakeGuest()
        {
            ((UserShopperHarmony)_shopper).MakeGuest();
        }
        public MarketAnswer OpenStore(string storeName, string address)
        {
            OpenStoreSlave slave = new OpenStoreSlave(_shopper, storeDL);
            Store S = slave.OpenStore(storeName, address);
            if (S!=null)
                stores.AddLast(S);
            return slave.Answer;
        }
        
        public MarketAnswer ViewStoreInfo(string store)
        {
            ViewStoreInfoSlave slave = new ViewStoreInfoSlave(_shopper, storeDL);
            slave.ViewStoreInfo(store);
            return slave.answer;
        }
     
        
        public MarketAnswer ViewStoreStock(string storename)
        {
            ViewStoreStockSlave slave = new ViewStoreStockSlave(_shopper, storeDL);
            slave.ViewStoreStock(storename);
            return slave.answer;
        }

	    public MarketAnswer ViewStoreStockAll(string storename)
	    {
		    ViewStoreStockSlave slave = new ViewStoreStockSlave(_shopper, storeDL);
		    slave.ViewStoreStockAll(storename);
		    return slave.answer;
	    }
	
		public MarketAnswer AddProductToCart(string store, string productName, int quantity)
        {
            AddProductToCartSlave slave = new AddProductToCartSlave(_shopper, storeDL);
            slave.AddProductToCart(store, productName, quantity);
            return slave.answer;
        }

    }
}
 