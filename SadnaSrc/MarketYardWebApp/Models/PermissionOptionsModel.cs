using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketYardWebApp.Models;

namespace MarketYardWebApp.Models
{
	public class PermissionOptionsModel : StoreItemModel
	{
		public PermissionsItem[] Items;

		public PermissionOptionsModel(int systemId, string state, string message, string store, string[] options) : base(systemId, state,
			message,store)
		{
			Items = new PermissionsItem[options.Length];
			for (int i = 0; i < Items.Length; i++)
			{
				Items[i] = new PermissionsItem(options[i]);
			}
		}

		public class PermissionsItem
		{
			public string Name;

			public PermissionsItem(string data)
			{
				Name = data;

			}
		}
	}
}
