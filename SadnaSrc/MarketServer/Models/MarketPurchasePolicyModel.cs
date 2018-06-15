using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketServer.Models;
using Microsoft.AspNetCore.Rewrite.Internal.ApacheModRewrite;

namespace MarketWeb.Models
{
    public class MarketPurchasePolicyModel:UserModel
    {
	    public string[] Operators;
	    public MarketPurchasePolicyItemModel[] Conditions;
        public string toBeSavedPolicy;

	    public MarketPurchasePolicyModel(int systemId, string state, string message, string[] operators, string[] conditionStrings) : base(systemId, state, message)
	    {
		    Operators = operators;
		    Conditions = new MarketPurchasePolicyItemModel[conditionStrings.Length];
	        for (int i = 0; i < conditionStrings.Length; i++)
	        {
	            Conditions[i] = new MarketPurchasePolicyItemModel(conditionStrings[i]);
	            if (i == conditionStrings.Length - 1)
	                toBeSavedPolicy = Conditions[i].data;
	        }

        }

        public class MarketPurchasePolicyItemModel
        {
            public string data;
            public string subject;
            public string type;

            public MarketPurchasePolicyItemModel(string newData)
            {
                data = newData;
                string[] sp = newData.Split('|');
                subject = sp[1];
                type = sp[2];
            }
        }
    }
}
