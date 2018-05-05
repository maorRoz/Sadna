using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketServer.Models;

namespace MarketWeb.Models
{
    public class StoreDetailsModel:UserModel
    {
	    private StoreItemModel[] Items { get; set; }
		public StoreDetailsModel(int systemId, string state, string message, string[] itemData) : base(systemId, state, message)
	    {
		    Items = new StoreItemModel[itemData.Length];
		    for (var i = 0; i < Items.Length; i++)
		    {
			    Items[i] = new StoreItemModel(itemData[i]);
		    }
		}

	    private class StoreItemModel
		{
			private string Name { get; set; }
			private string Address { get; set; }

		    public StoreItemModel(string data)
		    {
			    var dataParam = data.Split(new[] { "Name : ", " Address : "}, StringSplitOptions.RemoveEmptyEntries);
			    Name = dataParam[0];
			    Address = dataParam[1];
		    }
	    }
	}
}
