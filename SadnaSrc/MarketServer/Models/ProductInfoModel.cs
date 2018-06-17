using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketServer.Models;

namespace MarketWeb.Models
{
    public class ProductInfoModel: UserModel
	{
		public string Store;
		public string Product;
		public ProductInfo Item;

		public ProductInfoModel(int systemId, string state, string message, string store, string product, string data) : base(systemId, state, message)
		{
			Store = store;
			Product = product;
			Item = new ProductInfo(data);
		}

		public class ProductInfo
		{
			public string Name { get; set; }
			public string Price { get; set; }
			public string Description { get; set; }

			public ProductInfo(string data)
			{
				var dataParam = data.Split(new[] { " name: ", " base price: ", " description: "}, StringSplitOptions.RemoveEmptyEntries);
				Name = dataParam[0];
				Price = dataParam[1];
				Description = dataParam[2];
			}
		}

	}
}
