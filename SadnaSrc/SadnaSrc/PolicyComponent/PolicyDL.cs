using SadnaSrc.Main;
using System.Collections.Generic;


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
            policy.IsRoot = true;
            Save_NonRoot_Policy(policy);
        }
        public void RemovePolicy(PurchasePolicy policy)
        {
            if (policy != null)
            {
                if (policy is Operator)
                    RemovePolicy((Operator) policy);
                else
                    RemovePolicy((Condition) policy);
            }
        }
        public PurchasePolicy GetPolicy(PolicyType type, string subject)
        {
            PurchasePolicy policy = SelectPolicyFromOperatorTable(type, subject);
            if (policy == null)
                policy = SelectPolicyFromConditionTable(type, subject);
            if (policy != null)
                policy.IsRoot = true;
            return policy;
        }
        public List<PurchasePolicy> GetAllPolicies()
        {
            List<int> listOfIDs = new List<int>();
            List<PurchasePolicy> result = new List<PurchasePolicy>();
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("Operator", "SystemID", "isRoot = 'true'"))
            {
                while (dbReader.Read())
                {
                    listOfIDs.Add(dbReader.GetInt32(0));
                }
            }
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("conditions", "SystemID", "isRoot = 'true'"))
            {
                while (dbReader.Read())
                {
                    listOfIDs.Add(dbReader.GetInt32(0));
                }
            }

            foreach (int idNumber in listOfIDs)
            {
                PurchasePolicy policy = GetPolicyById(idNumber);
                policy.IsRoot = true;
                result.Add(policy);
            }
            return result;
        }
        private void Save_NonRoot_Policy(PurchasePolicy policy)
        {
            if (policy != null)
            {
                if (policy is Operator)
                    SavePolicy((Operator) policy);
                else
                    SavePolicy((Condition) policy);
            }
        }
        private void SavePolicy(Operator policy)
        {
            string fields = "SystemID,OperatorType,PolicyType,Subject,COND1ID,COND2ID,isRoot";
            dbConnection.InsertTable("Operator", fields,
                new []{"@idParam","@typeParam","@policyParam","@subjectParam","@cond1Param","@cond2Param","@rootParam"},
                policy.GetPolicyValuesArray());
                Save_NonRoot_Policy(policy._cond1);
                Save_NonRoot_Policy(policy._cond2);
        }
        private void SavePolicy(Condition policy)
        {
            string fields = "SystemID,conditionsType,PolicyType,Subject,value,isRoot";
            dbConnection.InsertTable("conditions", fields,
                new []{"@idParam","@conditionParam","@typeParam","@subjectParam","@valueParam","@rootParam"},
                policy.GetPolicyValuesArray());
        }

        
        private void RemovePolicy(Operator policy)
        {
            dbConnection.DeleteFromTable("Operator", "SystemID = '" + policy.ID + "'");
            RemovePolicy(policy._cond1);
            RemovePolicy(policy._cond2);
        }
        private void RemovePolicy(Condition policy)
        {
            dbConnection.DeleteFromTable("conditions", "SystemID = '" + policy.ID + "'");
        }
        
        private PurchasePolicy SelectPolicyFromOperatorTable(PolicyType type, string subject)
        {
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("Operator", "*", "PolicyType = '" + PurchasePolicy.PrintEnum(type) + "' AND " +
                                                                                     " Subject = '" + subject + "' AND isRoot = 'true'"))
            {
                while (dbReader.Read())
                {
                    int id = dbReader.GetInt32(0);
                    string operatorType = dbReader.GetString(1);
                    string policyType = dbReader.GetString(2);
                    string s = dbReader.GetString(3);
                    int cond1Id = dbReader.GetInt32(4);
                    int cond2Id = dbReader.GetInt32(5);
                    PurchasePolicy cond1OptionalNull = null;
                    PurchasePolicy cond2OptionalNull = null;
                    string subjectOptionalNull = null;
                    if (cond1Id != -1)
                        cond1OptionalNull = GetPolicyById(cond1Id);
                    if (cond2Id != -1)
                        cond2OptionalNull = GetPolicyById(cond2Id);
                    if (s != "'NULL'")
                        subjectOptionalNull = s;
                    if (operatorType == "AndOperator")
                        return new AndOperator(PurchasePolicy.GetEnumFromStringValue(policyType), subjectOptionalNull, cond1OptionalNull, cond2OptionalNull, id);
                    if (operatorType == "OrOperator")
                        return new OrOperator(PurchasePolicy.GetEnumFromStringValue(policyType), subjectOptionalNull, cond1OptionalNull, cond2OptionalNull, id);
                    if (operatorType == "NotOperator")
                        return new NotOperator(PurchasePolicy.GetEnumFromStringValue(policyType), subjectOptionalNull, cond1OptionalNull, cond2OptionalNull, id);
                }
            }

            return null;
        }

        private PurchasePolicy SelectPolicyFromConditionTable(PolicyType type, string subject)
        {
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("conditions", "*",
                    "PolicyType = '" + PurchasePolicy.PrintEnum(type) + "' AND " +
                    " Subject = '" + subject + "' AND isRoot = 'true'"))
            {
                while (dbReader.Read())
                {

                    int id = dbReader.GetInt32(0);
                    string conditionType = dbReader.GetString(1);
                    string policyType = dbReader.GetString(2);
                    string s = dbReader.GetString(3);
                    string value = dbReader.GetString(4);
                    string subjectOptionalNull = null;
                    if (s != "'NULL'")
                        subjectOptionalNull = s;
                    string valueOptionalNull = null;
                    if (value!="'NULL'")
                        valueOptionalNull = value;
                    switch (conditionType)
                    {
                        case "AddressEquals":
                            return new AddressEquals(PurchasePolicy.GetEnumFromStringValue(policyType), subjectOptionalNull, valueOptionalNull, id);
                        case "PriceGreaterThan":
                            return new PriceGreaterThan(PurchasePolicy.GetEnumFromStringValue(policyType), subjectOptionalNull, valueOptionalNull, id);
                        case "PriceLessThan":
                            return new PriceLessThan(PurchasePolicy.GetEnumFromStringValue(policyType), subjectOptionalNull, valueOptionalNull, id);
                        case "QuantityGreaterThan":
                            return new QuantityGreaterThan(PurchasePolicy.GetEnumFromStringValue(policyType), subjectOptionalNull, valueOptionalNull, id);
                        case "QuantityLessThan":
                            return new QuantityLessThan(PurchasePolicy.GetEnumFromStringValue(policyType), subjectOptionalNull, valueOptionalNull, id);
                        case "UsernameEquals":
                            return new UsernameEquals(PurchasePolicy.GetEnumFromStringValue(policyType), subjectOptionalNull, valueOptionalNull, id);
                    }
                }
            }

            return null;
        }
        private PurchasePolicy GetPolicyById(int systemid)
        {
            PurchasePolicy policy = SelectPolicyFromOperatorTable(systemid);
            if (policy == null)
            {
                policy = SelectPolicyFromConditionTable(systemid);
            }
            return policy;
        }

        private PurchasePolicy SelectPolicyFromConditionTable(int systemid)
        {
            {
                using (var dbReader =
                    dbConnection.SelectFromTableWithCondition("conditions", "*",
                        "SystemID = '"+ systemid + "'"))
                {
                    while (dbReader.Read())
                    {
                        int id = dbReader.GetInt32(0);
                        string conditionType = dbReader.GetString(1);
                        string policyType = dbReader.GetString(2);
                        string subject = dbReader.GetString(3);
                        string value = dbReader.GetString(4);
                        string subjectOptionalNull = null;
                        if (subject != "'NULL'")
                            subjectOptionalNull = subject;
                        string valueOptionalNull = null;
                        if (value != "'NULL'")
                            valueOptionalNull = value;
                        if (conditionType == "AddressEquals")
                            return new AddressEquals(PurchasePolicy.GetEnumFromStringValue(policyType), subjectOptionalNull, valueOptionalNull,
                                id);
                        if (conditionType == "PriceGreaterThan")
                            return new PriceGreaterThan(PurchasePolicy.GetEnumFromStringValue(policyType), subjectOptionalNull,
                                valueOptionalNull, id);
                        if (conditionType == "PriceLessThan")
                            return new PriceLessThan(PurchasePolicy.GetEnumFromStringValue(policyType), subjectOptionalNull, valueOptionalNull,
                                id);
                        if (conditionType == "QuantityGreaterThan")
                            return new QuantityGreaterThan(PurchasePolicy.GetEnumFromStringValue(policyType), subjectOptionalNull,
                                valueOptionalNull, id);
                        if (conditionType == "QuantityLessThan")
                            return new QuantityLessThan(PurchasePolicy.GetEnumFromStringValue(policyType), subjectOptionalNull,
                                valueOptionalNull, id);
                        if (conditionType == "UsernameEquals")
                            return new UsernameEquals(PurchasePolicy.GetEnumFromStringValue(policyType), subjectOptionalNull, valueOptionalNull,
                                id);
                    }
                }

                return null;
            }
        }

        private PurchasePolicy SelectPolicyFromOperatorTable(int systemid)
        {

            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("Operator", "*", "SystemID = '" + systemid + "'"))
            {
                while (dbReader.Read())
                {
                    int id = dbReader.GetInt32(0);
                    string operatorType = dbReader.GetString(1);
                    string policyType = dbReader.GetString(2);
                    string subject = dbReader.GetString(3);
                    int cond1Id = dbReader.GetInt32(4);
                    int cond2Id = dbReader.GetInt32(5);
                    PurchasePolicy cond1OptionalNull = null;
                    PurchasePolicy cond2OptionalNull = null;
                    string subjectOptionalNull = null;
                    if (cond1Id != -1)
                        cond1OptionalNull = GetPolicyById(cond1Id);
                    if (cond2Id != -1)
                        cond2OptionalNull = GetPolicyById(cond2Id);
                    if (subject != "'NULL'")
                        subjectOptionalNull = subject;
                    if (operatorType == "AndOperator")
                        return new AndOperator(PurchasePolicy.GetEnumFromStringValue(policyType), subjectOptionalNull, cond1OptionalNull, cond2OptionalNull, id);
                    if (operatorType == "OrOperator")
                        return new OrOperator(PurchasePolicy.GetEnumFromStringValue(policyType), subjectOptionalNull, cond1OptionalNull, cond2OptionalNull, id);
                    if (operatorType == "NotOperator")
                        return new NotOperator(PurchasePolicy.GetEnumFromStringValue(policyType), subjectOptionalNull, cond1OptionalNull, cond2OptionalNull, id);
                }
            }
            return null;
        }

    }
}

