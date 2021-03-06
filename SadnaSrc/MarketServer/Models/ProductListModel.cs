﻿using MarketServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketWeb.Models
{
    public class ProductListModel : UserModel
	{
		public ProductItem[] Items { get; set; }
		public ProductListModel(int systemId, string state, string message, string[] itemData) : base(systemId, state, message)
		{
			Items = new ProductItem[itemData.Length];
			for (int i = 0; i < Items.Length; i++)
			{
				Items[i] = new ProductItem(itemData[i]);
			}
		}

		public class ProductItem
		{
			public string Name { get; set; }
			public string Price { get; set; }
			public string Description { get; set; }
			public string PurchaseWay { get; set; }
			public string Quantity { get; set; }
			public string Discount { get; set; }
			public string Store { get; set; }

			public ProductItem(string data)
			{
				var dataParam = data.Split(new[] { " name: ", " base price: ", " description: ", " Discount: {", "}", " Purchase Way: ", " Quantity: ", " Store: "}, StringSplitOptions.RemoveEmptyEntries);
				Name = dataParam[0];
				Price = dataParam[1];
				Description = dataParam[2];
				Discount = dataParam[3];
				PurchaseWay = dataParam[4];
				Quantity = dataParam[5];
				Store = dataParam[6];
			}
		}
	}
}
