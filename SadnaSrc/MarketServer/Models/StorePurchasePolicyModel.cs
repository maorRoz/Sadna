using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketWeb.Models
{
    public class StorePurchasePolicyModel:StoreItemModel
    {
        public string[] Operators;
        public StorePurchasePolicyItemModel[] Conditions;


        public StorePurchasePolicyModel(int systemId, string state, string message,string store, string[] operators, string[] conditionStrings) : base(systemId, state, message, store)
        {
            Operators = operators;
            Conditions = new StorePurchasePolicyItemModel[conditionStrings.Length];
            for (int i = 0; i < conditionStrings.Length; i++)
                Conditions[i] = new StorePurchasePolicyItemModel(conditionStrings[i]);
        }

        public class StorePurchasePolicyItemModel
        {
            public string data;
            public string subject;
            public string type;
            public string optProd;
            public StorePurchasePolicyItemModel(string newData)
            {
                data = newData;
                string[] sp = newData.Split('|');
                subject = sp[1];
                type = sp[2];
            }
        }
    }
}
