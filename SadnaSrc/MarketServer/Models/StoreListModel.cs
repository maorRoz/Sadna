using MarketServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketWeb.Models
{
    public class StoreListModel : UserModel
	{
		public StoreItemModel[] Items { get; set; }
		public StoreListModel(int systemId, string state, string[] itemData) : base(systemId, state,null)
		{
			Items = new StoreItemModel[itemData.Length];
			for (int i = 0; i < Items.Length; i++)
			{
				Items[i] = new StoreItemModel(itemData[i]);
			}
		}

		public class StoreItemModel
		{
			public string Name { get; set; }

			public StoreItemModel(string data)
			{
				Name = data;
			}
		}
	}
}
