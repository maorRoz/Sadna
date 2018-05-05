using MarketServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketWeb.Models
{
    public class StoreListModel : UserModel
	{
		public StoreItem[] Items { get; set; }
		public StoreListModel(int systemId, string state, string[] itemData) : base(systemId, state,null)
		{
			Items = new StoreItem[itemData.Length];
			for (int i = 0; i < Items.Length; i++)
			{
				Items[i] = new StoreItem(itemData[i]);
			}
		}

		public class StoreItem
		{
			public string Name { get; set; }

			public StoreItem(string data)
			{
				Name = data;
			}
		}
	}
}
