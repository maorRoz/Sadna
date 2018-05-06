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

        private static PolicyHandler _instance;

        public static PolicyHandler Instance => _instance ?? (_instance = new PolicyHandler());

        private PolicyHandler()
        {
            policies = new List<PurchasePolicy>();
        }

        public PurchasePolicy CreatePolicy(PolicyType type, string subject, OperatorType op, PurchasePolicy cond1,
            PurchasePolicy cond2)
        {
            PurchasePolicy policy = null;
            switch (op)
            {
                case OperatorType.AND:
                    policy = new AndOperator(type, subject, cond1, cond2);
                    break;
                case OperatorType.OR:
                    policy = new OrOperator(type, subject, cond1, cond2);
                    break;
                case OperatorType.NOT:
                    policy = new NotOperator(type, subject, cond1, null);
                    break;
            }

            return policy;
        }

        public PurchasePolicy CreateCondition(PolicyType type, string subject, ConditionType cond, string value)
        {
            PurchasePolicy policy = null;
            switch (cond)
            {
                case ConditionType.AddressEqual:
                    policy = new AddressEquals(type, subject, value);
                    break;
                case ConditionType.PriceGreater:
                    policy = new PriceGreaterThan(type, subject, value);
                    break;
                case ConditionType.PriceLesser:
                    policy = new PriceLessThan(type, subject, value);
                    break;
                case ConditionType.QuantityGreater:
                    policy = new QuantityGreaterThan(type, subject, value);
                    break;
                case ConditionType.QuantityLesser:
                    policy = new QuantityLessThan(type, subject, value);
                    break;
                case ConditionType.UsernameEqual:
                    policy = new UsernameEquals(type, subject, value);
                    break;
            }

            return policy;
        }

        public void AddPolicy(PurchasePolicy policy)
        {
            policies.Add(policy);
        }

        public void RemovePolicy(PolicyType type, string subject)
        {
            foreach (PurchasePolicy policy in policies)
            {
                if (policy._type == type && policy._subject == subject)
                    policies.Remove(policy);
            }
        }

        public bool CheckPolicy(PolicyType type, string subject, string username, string address, int quantity, double price)
        {
            PurchasePolicy policy = GetPolicy(type, subject);
            if (policy == null) return false;
            return policy.Evaluate(username, address, quantity, price);
        }

        public PurchasePolicy GetPolicy(PolicyType type, string subject)
        {
            foreach (PurchasePolicy policy in policies)
            {
                if (policy._type == type && policy._subject == subject)
                    return policy;
            }

            return null;
        }
    }
}
