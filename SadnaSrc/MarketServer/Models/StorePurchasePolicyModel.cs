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
        public string toBeSavedPolicy;

        public StorePurchasePolicyModel(int systemId, string state, string message,string store, string[] operators, string[] conditionStrings) : base(systemId, state, message, store)
        {
            Operators = operators;
            Conditions = new StorePurchasePolicyItemModel[conditionStrings.Length];
            for (int i = 0; i < conditionStrings.Length; i++)
            {
                Conditions[i] = new StorePurchasePolicyItemModel(conditionStrings[i]);
                if (i == conditionStrings.Length - 1)
                    toBeSavedPolicy = Conditions[i].data;
            }
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
                string[] subjectSplit = sp[1].Split('.');
                subject = subjectSplit[0];
                if (subjectSplit.Length > 1) 
                    optProd = subjectSplit[1];
                type = sp[2];
            }
        }
    }
}
