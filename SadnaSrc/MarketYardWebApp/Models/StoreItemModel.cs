using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketYardWebApp.Models;

namespace MarketYardWebApp.Models
{
    public class StoreItemModel : UserModel
    {
	    public string Name { get; set; }

		public StoreItemModel(int systemId, string state, string message, string store) : base(systemId, state, message)
		{
			Name = store;
		}
	}
}
