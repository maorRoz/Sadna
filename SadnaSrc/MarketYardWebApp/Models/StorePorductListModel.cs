using MarketYardWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketYardWebApp.Models
{
    public class StorePorductListModel : UserModel
	{
        public string StoreName { get; set; }
		public ProductItem[] Items { get; set; }
		public StorePorductListModel(int systemId, string state,string message, string store,string[] itemData) : base(systemId, state, message)
		{
		    StoreName = store;
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
            public string Discount { get; set; }
            public string PurchaseWay { get; set; }
            public string Quantity { get; set; }
            public ProductItem(string data)
			{
			    var dataParam = data.Split(new[] { " name: ", " base price: ", " description: ", " Discount: {","}", " Purchase Way: ", " Quantity: " }, StringSplitOptions.RemoveEmptyEntries);
			    Name = dataParam[0];
			    Price = dataParam[1];
			    Description = dataParam[2];
			    Discount = dataParam[3];//fix 
			    PurchaseWay = dataParam[4];
			    Quantity = dataParam[5];
			}
		}
	}
}
