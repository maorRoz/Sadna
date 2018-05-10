using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public interface IPolicyHandler
    {
        void AddPolicy(int policyId);
        void RemovePolicy(PolicyType type, string subject);
        void RemoveSessionPolicy(int policyId);
        int[] GetSessionPolicies();
        string[] GetSessionPoliciesStrings();
        string[] GetPolicyData(PolicyType type, string subject);

        string[] PolicyTypeStrings();
        string[] OperatorTypeStrings();
        string[] ConditionTypeStrings();
    }

    public enum PolicyType
    {
        Global,
        Store,
        StockItem,
        Product,
        Category
    };

    public enum OperatorType
    {
        AND,
        OR,
        NOT
    };

    public enum ConditionType
    {
        PriceGreater,
        PriceLesser,
        QuantityGreater,
        QuantityLesser,
        UsernameEqual,
        AddressEqual
    };
}
