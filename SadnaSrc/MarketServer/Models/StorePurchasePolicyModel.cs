using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketWeb.Models
{
    public class StorePurchasePolicyModel:StoreItemModel
    {
	    public string[] Operators;
	    public string[] Conditions;
		public StorePurchasePolicyModel(int systemId, string state, string message, string store, string[] operators, string[] conditions) : base(systemId, state, message, store)
	    {
		    Operators = operators;
		    Conditions = conditions;
	    }
    }
}
