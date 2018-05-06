using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public interface IPolicyHandler
    {
        PurchasePolicy CreatePolicy(PolicyType type, string subject, OperatorType op, PurchasePolicy cond1, PurchasePolicy cond2);
        PurchasePolicy CreateCondition(PolicyType type, string subject, ConditionType cond, string value);
        PurchasePolicy CreateStockItemCondition(string store, string product, ConditionType cond, string value);
        PurchasePolicy GetPolicy(PolicyType type, string subject);
        void AddPolicy(PurchasePolicy policy);
        void RemovePolicy(PolicyType type, string subject);
        bool CheckRelevantPolicies(string product, string store, string category, string username, string address, int quantity, double price);

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
