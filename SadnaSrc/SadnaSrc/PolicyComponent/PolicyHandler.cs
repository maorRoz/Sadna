using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public class PolicyHandler : IPolicyHandler
    {
        public List<PurchasePolicy> policies;
        private List<PurchasePolicy> SessionPolicies;

        private static PolicyHandler _instance;
        private static IPolicyDL _dataLayer;

        public static PolicyHandler Instance => _instance ?? (_instance = new PolicyHandler());

        private PolicyHandler()
        {
            policies = new List<PurchasePolicy>();
        }

        public string[] CreatePolicy(PolicyType type, string subject, OperatorType op, int id1, int id2)
        {
            PurchasePolicy policy = null;
            switch (op)
            {
                case OperatorType.AND:
                    policy = new AndOperator(type, subject, GetPolicy(id1), GetPolicy(id2), SessionPolicies.Count);
                    break;
                case OperatorType.OR:
                    policy = new OrOperator(type, subject, GetPolicy(id1), GetPolicy(id2), SessionPolicies.Count);
                    break;
                case OperatorType.NOT:
                    policy = new NotOperator(type, subject, GetPolicy(id1), null, SessionPolicies.Count);
                    break;
            }
            SessionPolicies.Add(policy);
            return policy.GetData();
        }

        public string[] CreateCondition(PolicyType type, string subject, ConditionType cond, string value)
        {
            PurchasePolicy policy = null;
            switch (cond)
            {
                case ConditionType.AddressEqual:
                    policy = new AddressEquals(type, subject, value, SessionPolicies.Count);
                    break;
                case ConditionType.PriceGreater:
                    policy = new PriceGreaterThan(type, subject, value, SessionPolicies.Count);
                    break;
                case ConditionType.PriceLesser:
                    policy = new PriceLessThan(type, subject, value, SessionPolicies.Count);
                    break;
                case ConditionType.QuantityGreater:
                    policy = new QuantityGreaterThan(type, subject, value, SessionPolicies.Count);
                    break;
                case ConditionType.QuantityLesser:
                    policy = new QuantityLessThan(type, subject, value, SessionPolicies.Count);
                    break;
                case ConditionType.UsernameEqual:
                    policy = new UsernameEquals(type, subject, value, SessionPolicies.Count);
                    break;
            }
            SessionPolicies.Add(policy);
            return policy.GetData();
        }

        public string[] CreateStockItemCondition(string store, string product, ConditionType cond, string value)
        {
            return CreateCondition(PolicyType.StockItem, store + "." + product, cond, value);
        }

        public void AddPolicy(int policyId)
        {
            PurchasePolicy toAdd = GetPolicy(policyId);
            toAdd.ID = GeneratePolicyID();
            policies.Add(toAdd);
            SessionPolicies.Clear();
            //_dataLayer.SavePolicy(policy);
        }

        public void RemovePolicy(PolicyType type, string subject)
        {
            foreach (PurchasePolicy policy in policies)
            {
                if (policy.Type == type && policy.Subject == subject)
                {
                    policies.Remove(policy);
                    //_dataLayer.RemovePolicy(policy);
                }
                    
            }
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
            foreach (PurchasePolicy policy in policies)
            {
                if (policy.Type == type && policy.Subject == subject)
                    return policy;
            }

            return null;
        }

        private PurchasePolicy GetPolicy(int id)
        {
            foreach (PurchasePolicy policy in policies)
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
    }
}
