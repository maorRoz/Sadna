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

        public void SavePolicy(PurchasePolicy policy)
        {
            if (policy is Operator)
                SavePolicy((Operator)policy);
            else
                SavePolicy((Condition)policy);
        }

        private void SavePolicy(Operator policy)
        {
            string fields = "SystemID,ConditionType,PolicyType,Subject,COND1ID,COND2ID";
            dbConnection.InsertTable("Operator", fields,
                policy.GetPolicyStringValues(), policy.GetPolicyValuesArray());
                SavePolicy(policy._cond1);
                SavePolicy(policy._cond2);
        }
        private void SavePolicy(Condition policy)
        {
            string fields = "SystemID,ConditionType,PolicyType,Subject,value";
            dbConnection.InsertTable("Condition", fields,
                policy.GetPolicyStringValues(), policy.GetPolicyValuesArray());
        }

        public void RemovePolicy(PurchasePolicy policy)
        {
            if (policy is Operator)
                RemovePolicy((Operator)policy);
            else
                RemovePolicy((Condition)policy);
        }
        private void RemovePolicy(Operator policy)
        {
            dbConnection.DeleteFromTable("Operator", "SystemID = '" + policy.ID + "'");
            RemovePolicy(policy._cond1);
            RemovePolicy(policy._cond2);
        }
        private void RemovePolicy(Condition policy)
        {
            dbConnection.DeleteFromTable("Condition", "SystemID = '" + policy.ID + "'");
        }

        public PurchasePolicy GetPolicy(int wantedid)
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

