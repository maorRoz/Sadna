using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketServer.Models;

namespace MarketWeb.Models
{
    public class StoreDetailsModel:UserModel
    {
	    public string Name { get; set; }
	    public string Address { get; set; }

	    public StoreDetailsModel(int systemId, string state, string message, string itemData) : base(systemId, state,
		    message)
	    {
		    var dataParam = itemData.Split(new[] {"Name : ", " Address : "}, StringSplitOptions.RemoveEmptyEntries);
		    Name = dataParam[0];
		    Address = dataParam[1];

	    }
    }
}
