using SadnaSrc.Main;
using System.Collections.Generic;
using Castle.Core.Internal;
using SadnaSrc.MarketData;


namespace SadnaSrc.PolicyComponent
{
    public class PolicyDL : IPolicyDL
    {
        private static PolicyDL _instance;

        public static PolicyDL Instance => _instance ?? (_instance = new PolicyDL());

        private readonly IMarketDB dbConnection;

        private PolicyDL()
        {
            dbConnection = new ProxyMarketDB();
        }
        //------------------
        public void SavePolicy(PurchasePolicy policy)
        {
            if (policy != null)
            {
                if (policy is Operator)
                    WritePolicyToDB((Operator)policy);
                else
                    WritePolicyToDB((Condition)policy);
            }
        }

        public void RemovePolicy(PurchasePolicy policy)
        {
            string subjectString = "NULL";
            if (!policy.Subject.IsNullOrEmpty())
                subjectString = policy.Subject;
            dbConnection.DeleteFromTable("ComplexPolicies", "PolicyType = '" + PurchasePolicy.PrintEnum(policy.Type) + "' AND Subject = '" + subjectString + "'");
            dbConnection.DeleteFromTable("SimplePolicies", "PolicyType = '" + PurchasePolicy.PrintEnum(policy.Type) + "' AND Subject = '" + subjectString + "'");
        }

        public PurchasePolicy GetPolicy(PolicyType type, string subject)
        {
            PurchasePolicy policy = FindComplexPolicy(-1, type, subject, true);
            if (policy == null)
                policy = FindSimplePolicy(-1, type, subject, true);
            if (policy != null)
                policy.IsRoot = true;
            return policy;
        }
        //--------------------

        public List<PurchasePolicy> GetAllPolicies()
        {
            List<int> listOfIDs = new List<int>();
            List<PurchasePolicy> result = new List<PurchasePolicy>();
            using (var dbReader = dbConnection.SelectFromTableWithCondition("ComplexPolicies", "*", "Root = 'true'"))
            {
                while (dbReader.Read())
                {
                    listOfIDs.Add(dbReader.GetInt32(0));
                    int id = dbReader.GetInt32(0);
                    PolicyType type = PurchasePolicy.GetEnumFromStringValue(dbReader.GetString(2));
                    string subject = dbReader.GetString(3);
                    PurchasePolicy policy = FindComplexPolicy(id, type, subject, true);
                    policy.IsRoot = true;
                    result.Add(policy);
                }
            }
            using (var dbReader = dbConnection.SelectFromTableWithCondition("SimplePolicies", "*", "Root = 'true'"))
            {
                while (dbReader.Read())
                {
                    int id = dbReader.GetInt32(0);
                    PolicyType type = PurchasePolicy.GetEnumFromStringValue(dbReader.GetString(2));
                    string subject = dbReader.GetString(3);
                    PurchasePolicy policy = FindSimplePolicy(id, type, subject, true);
                    policy.IsRoot = true;
                    result.Add(policy);
                }
            }
            return result;
        }

        private void WritePolicyToDB(Operator policy)
        {
            string fields = "SystemID,Operator,PolicyType,Subject,Cond1,Cond2,Root";
            dbConnection.InsertTable("ComplexPolicies", fields,
                new[]
                {
                    "@idParam", "@typeParam", "@policyParam", "@subjectParam", "@cond1Param", "@cond2Param",
                    "@rootParam"
                },
                GetPolicyValues(policy));
                SavePolicy(policy._cond1);
                SavePolicy(policy._cond2);
        }
        private void WritePolicyToDB(Condition policy)
        {
            string fields = "SystemID,Condition,PolicyType,Subject,Value,Root";
            dbConnection.InsertTable("SimplePolicies", fields,
                new []{"@idParam","@conditionParam","@typeParam","@subjectParam","@valueParam","@rootParam"},
                GetPolicyValues(policy));
        }

        
        private PurchasePolicy FindComplexPolicy(int policyID, PolicyType type, string subject, bool isRoot)
        {
            string cond = "";
            if (policyID != -1)
                cond = "SystemID = '" + policyID + "' AND ";
            cond += "PolicyType = '" + PurchasePolicy.PrintEnum(type);
            if (subject != null)
                cond += "' AND Subject = '" + subject + "'";
            if (isRoot)
                cond += " AND Root = 'true'";

            using (var dbReader = dbConnection.SelectFromTableWithCondition("ComplexPolicies", "*", cond))
            {
                while (dbReader.Read())
                {
                    int id = dbReader.GetInt32(0);
                    string op = dbReader.GetString(1);
                    int cond1Id = dbReader.GetInt32(4);
                    int cond2Id = dbReader.GetInt32(5);
                    PurchasePolicy cond1 = null;
                    PurchasePolicy cond2 = null;
                    if (cond1Id != -1)
                        cond1 = GetPolicy(cond1Id, type, subject);
                    if (cond2Id != -1)
                        cond2 = GetPolicy(cond2Id, type, subject);
                    switch (op)
                    {
                        case "AND":
                            return new AndOperator(type, subject, cond1, cond2, id);
                        case "OR":
                            return new OrOperator(type, subject, cond1, cond2, id);
                        case "NOT":
                            return new NotOperator(type, subject, cond1, null, id);
                    }
                }
            }

            return null;
        }

        private PurchasePolicy FindSimplePolicy(int policyID, PolicyType type, string subject, bool isRoot)
        {
            string cond = "";
            if (policyID != -1)
                cond = "SystemID = '" + policyID + "' AND ";
            cond += "PolicyType = '" + PurchasePolicy.PrintEnum(type);
            if (subject != null)
                cond += "' AND Subject = '" + subject + "'";
            if (isRoot)
                cond += " AND Root = 'true'";
            using (var dbReader = dbConnection.SelectFromTableWithCondition("SimplePolicies", "*", cond))
            {
                while (dbReader.Read())
                {

                    int id = dbReader.GetInt32(0);
                    string conditionType = dbReader.GetString(1);
                    string value = dbReader.GetString(4);
                    switch (conditionType)
                    {
                        case "AddressEquals":
                            return new AddressEquals(type, subject, value, id);
                        case "PriceGreaterThan":
                            return new PriceGreaterThan(type, subject, value, id);
                        case "PriceLessThan":
                            return new PriceLessThan(type, subject, value, id);
                        case "QuantityGreaterThan":
                            return new QuantityGreaterThan(type, subject, value, id);
                        case "QuantityLessThan":
                            return new QuantityLessThan(type, subject, value, id);
                        case "UsernameEquals":
                            return new UsernameEquals(type, subject, value, id);
                    }
                }
            }

            return null;
        }

        private PurchasePolicy GetPolicy(int systemid, PolicyType type, string subject)
        {
            PurchasePolicy policy = FindComplexPolicy(systemid, type, subject, false);
            if (policy == null)
                policy = FindSimplePolicy(systemid, type, subject, false);

            return policy;
        }

        private object[] GetPolicyValues(Operator policy)
        {
            string subject;
            if (policy.Subject == null)
                subject = "NULL";
            else
                subject = policy.Subject;
            int cond1 = policy._cond1.ID;
            int cond2;
            if (policy._cond2 == null)
                cond2 = -1;
            else
                cond2 = policy._cond2.ID;
            return new object[]
            {
                policy.ID,
                policy.GetMyType(),
                PurchasePolicy.PrintEnum(policy.Type),
                subject,
                cond1,
                cond2,
                PurchasePolicy.PrintBoolean(policy.IsRoot)
            };
        }

        private object[] GetPolicyValues(Condition policy)
        {

            string subject;
            if (policy.Subject == null)
                subject = "NULL";
            else
                subject = policy.Subject;

            string value = policy.Value;

            return new object[]
            {
                policy.ID,
                policy.GetMyType(),
                PurchasePolicy.PrintEnum(policy.Type),
                subject,
                value,
                PurchasePolicy.PrintBoolean(policy.IsRoot)
            };
        }
    }
}

