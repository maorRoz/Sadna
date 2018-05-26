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
		public StoreListModel(int systemId, string state, string[] itemData,string message) : base(systemId, state,message)
		{
			Items = new StoreItemModel[itemData.Length];
			for (int i = 0; i < Items.Length; i++)
			{
				Items[i] = new StoreItemModel(systemId,state,null,itemData[i]);
			}
		}
	}
}
