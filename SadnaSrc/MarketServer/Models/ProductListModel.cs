using MarketServer.Models;
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

			public ProductItem(string data)
			{
				Name = data;
			}
		}
	}
}
