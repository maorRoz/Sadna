using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.PolicyComponent
{
    public class PolicyDL : IPolicyDL
    {
        private static PolicyDL _instance;

        public static PolicyDL Instance => _instance ?? (_instance = new PolicyDL());

        private MarketDB dbConnection;

        private PolicyDL()
        {
            dbConnection = MarketDB.Instance;
        }

        public void SavePolicy(Operator policy)
        {


            string fields = "SystemID,ConditionType,PolicyType,Subject,COND1ID,COND2ID";
            dbConnection.InsertTable("Operator", fields,
                policy.GetPolicyStringValues(), policy.GetPolicyValuesArray());
        }
        public void SavePolicy(Condition policy)
        {
            string fields = "SystemID,ConditionType,PolicyType,Subject,value";
            dbConnection.InsertTable("Condition", fields,
                policy.GetPolicyStringValues(), policy.GetPolicyValuesArray());
        }

        public void RemovePolicy(PurchasePolicy policy)
        {
            throw new NotImplementedException();
        }

        public PurchasePolicy GetPolicy(PolicyType type, string subject)
        {
            throw new NotImplementedException();
        }

        public List<PurchasePolicy> GetAllPolicies()
        {
            throw new NotImplementedException();
        }

        public List<PurchasePolicy> GetPoliciesOfType(PolicyType type)
        {
            throw new NotImplementedException();
        }

        public void CleanSession()
        {
            throw new NotImplementedException();
        }
    }
}
