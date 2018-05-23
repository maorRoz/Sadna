﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketYardWebApp.Models;

namespace MarketYardWebApp.Models
{
    public class ProductInStoreModel:UserModel
    {
	    public string Store;
	    public string Product;
	    public ProductInStoreModel(int systemId, string state, string message, string store,string product) : base(systemId, state, message)
	    {
		    Store = store;
		    Product = product;
	    }
    }
}
