using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public interface IPolicyHandler
    {
        string[] CreatePolicy(OperatorType op, int id1, int id2);
        string[] CreateCondition(ConditionType cond, string value);
        void AddPolicy(int policyId);
        void RemovePolicy(PolicyType type, string subject);
        void RemoveSessionPolicy(int policyId);
        bool CheckRelevantPolicies(string product, string store, string category, string username, string address, int quantity, double price);
        int[] GetSessionPolicies();
        string[] GetPolicyData(PolicyType type, string subject);
        void StartSession(PolicyType type, string subject);
        void EndSession();

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
