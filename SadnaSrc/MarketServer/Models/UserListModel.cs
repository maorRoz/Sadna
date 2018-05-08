using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketServer.Models;

namespace MarketWeb.Models
{
    public class UserListModel : UserModel
    {
		public UserItem[] Items { get; set; }
		public UserListModel(int systemId, string state, string message, string[] itemData) : base(systemId, state, message)
		{
			Items = new UserItem[itemData.Length];
			for (int i = 0; i < Items.Length; i++)
			{
				Items[i] = new UserItem(itemData[i]);
			}
		}

		public class UserItem
		{
			public string Name { get; set; }

			public UserItem(string data)
			{
				Name = data;
			}
		}
	}
}
