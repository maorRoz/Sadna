using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public class PolicyHandler : IPolicyHandler
    {
        public List<PurchasePolicy> Policies;
        private List<PurchasePolicy> SessionPolicies;

        private static PolicyHandler _instance;
        private static IPolicyDL _dataLayer;

        public PolicyType Type;
        public string Subject;

        public static PolicyHandler Instance => _instance ?? (_instance = new PolicyHandler());

        private PolicyHandler()
        {
            Policies = new List<PurchasePolicy>();
            SessionPolicies = new List<PurchasePolicy>();
        }

        public string[] CreatePolicy(OperatorType op, int id1, int id2)
        {
            PurchasePolicy policy = null;
            switch (op)
            {
                case OperatorType.AND:
                    policy = new AndOperator(Type, Subject, GetPolicy(id1), GetPolicy(id2), SessionPolicies.Count);
                    break;
                case OperatorType.OR:
                    policy = new OrOperator(Type, Subject, GetPolicy(id1), GetPolicy(id2), SessionPolicies.Count);
                    break;
                case OperatorType.NOT:
                    policy = new NotOperator(Type, Subject, GetPolicy(id1), null, SessionPolicies.Count);
                    break;
            }
            SessionPolicies.Add(policy);
            return policy.GetData();
        }

        public string[] CreateCondition(ConditionType cond, string value)
        {
            PurchasePolicy policy = null;
            switch (cond)
            {
                case ConditionType.AddressEqual:
                    policy = new AddressEquals(Type, Subject, value, SessionPolicies.Count);
                    break;
                case ConditionType.PriceGreater:
                    policy = new PriceGreaterThan(Type, Subject, value, SessionPolicies.Count);
                    break;
                case ConditionType.PriceLesser:
                    policy = new PriceLessThan(Type, Subject, value, SessionPolicies.Count);
                    break;
                case ConditionType.QuantityGreater:
                    policy = new QuantityGreaterThan(Type, Subject, value, SessionPolicies.Count);
                    break;
                case ConditionType.QuantityLesser:
                    policy = new QuantityLessThan(Type, Subject, value, SessionPolicies.Count);
                    break;
                case ConditionType.UsernameEqual:
                    policy = new UsernameEquals(Type, Subject, value, SessionPolicies.Count);
                    break;
            }
            SessionPolicies.Add(policy);
            return policy.GetData();
        }

        public void AddPolicy(int policyId)
        {
            PurchasePolicy toAdd = GetPolicy(policyId);
            toAdd.ID = GeneratePolicyID();
            Policies.Add(toAdd);
            SessionPolicies.Clear();
            //_dataLayer.SavePolicy(policy);
        }

        public void RemovePolicy(PolicyType type, string subject)
        {
            PurchasePolicy toRemove = null;
            foreach (PurchasePolicy policy in Policies)
            {
                if (policy.Type == type && policy.Subject == subject)
                {
                    toRemove = policy;
                    //_dataLayer.RemovePolicy(policy);
                }
                    
            }
            Policies.Remove(toRemove);
        }

        public void RemoveSessionPolicy(int policyId)
        {
            PurchasePolicy toRemove = null;
            foreach (PurchasePolicy policy in SessionPolicies)
            {
                if (policy.ID == policyId)
                    toRemove = policy;
            }
            SessionPolicies.Remove(toRemove);
        }

        public bool CheckRelevantPolicies(string product, string store, string category, string username,
            string address, int quantity, double price)
        {
            return
            CheckPolicy(PolicyType.Global, null, username, address, quantity, price) &&
            CheckPolicy(PolicyType.Category, category, username, address, quantity, price) &&
            CheckPolicy(PolicyType.Product, product, username, address, quantity, price) &&
            CheckPolicy(PolicyType.Store, store, username, address, quantity, price) &&
            CheckPolicy(PolicyType.StockItem, store + "." + product, username, address, quantity, price);
        }

        public int[] GetSessionPolicies()
        {
            PurchasePolicy[] policiesArr = SessionPolicies.ToArray();
            int[] idArr = new int[policiesArr.Length];
            for (int i = 0; i < idArr.Length; i++)
                idArr[i] = policiesArr[i].ID;
            return idArr;
        }

        public string[] GetPolicyData(PolicyType type, string subject)
        {
            foreach (PurchasePolicy policy in Policies)
            {
                if (policy.Type == type && policy.Subject == subject)
                    return policy.GetData();
            }

            return null;
        }

        public string[] PolicyTypeStrings()
        {
            return new[] {"Global", "Store", "Stock Item", "Product", "Category"};
        }

        public string[] OperatorTypeStrings()
        {
            return new[] {"AND", "OR", "NOT"};
        }

        public string[] ConditionTypeStrings()
        {
            return new[] {"Price >=", "Price <=", "Quantity >=", "Quantity <=", "Username =", "Address ="};
        }

        private PurchasePolicy GetPolicy(PolicyType type, string subject)
        {
            foreach (PurchasePolicy policy in Policies)
            {
                if (policy.Type == type && policy.Subject == subject)
                    return policy;
            }

            return null;
        }

        private PurchasePolicy GetPolicy(int id)
        {
            foreach (PurchasePolicy policy in SessionPolicies)
            {
                if (policy.ID == id)
                    return policy;
            }

            return null;
        }

        private bool CheckPolicy(PolicyType type, string subject, string username, string address, int quantity, double price)
        {
            PurchasePolicy policy = GetPolicy(type, subject);
            if (policy == null) return true;
            return policy.Evaluate(username, address, quantity, price);
        }

        private int GeneratePolicyID()
        {
            Random random = new Random();
            var newID = random.Next(1000, 10000);
            while (GetPolicy(newID) != null)
            {
                newID = random.Next(1000, 10000);
            }

            return newID;
        }

        public void StartSession(PolicyType type, string subject)
        {
            Type = type;
            Subject = subject;
        }

        public void EndSession()
        {
            Type = PolicyType.Global;
            Subject = null;
            SessionPolicies.Clear();
        }

        public void CleanSession()
        {
            Policies.Clear();
            SessionPolicies.Clear();
            //_dataLayer.CleanSession();
        }
    }
}
