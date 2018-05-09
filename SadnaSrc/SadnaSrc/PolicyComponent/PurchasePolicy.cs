using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.StoreCenter;

namespace SadnaSrc.PolicyComponent
{
    public abstract class PurchasePolicy
    {
        public int ID;
        public readonly PolicyType Type;
        public readonly string Subject;
        public bool IsRoot;

        public PurchasePolicy(PolicyType type, string subject, int id)
        {
            ID = id;
            Type = type;
            Subject = subject;
            IsRoot = false;
        }

        public abstract bool Evaluate(string username, string address, int quantity, double price);

        public abstract string[] GetData();
        public static string PrintEnum(PolicyType type)
        {
            switch (type)
            {
                case PolicyType.Product: return "Product";
                case PolicyType.Category: return "Category";
                case PolicyType.Global: return "Global";
                case PolicyType.StockItem: return "StockItem";
                case PolicyType.Store: return "Store";
                default: throw new StoreException(MarketError.LogicError, "Enum value not exists");
            }
        }
        public static PolicyType GetEnumFromStringValue(string type)
        {
            switch (type)
            {
                case "Product": return PolicyType.Product;
                case "Category": return PolicyType.Category;
                case "Global": return PolicyType.Global;
                case "StockItem": return PolicyType.StockItem;
                case "Store": return PolicyType.Store;
                default: throw new StoreException(MarketError.LogicError, "Enum value not exists");
            }
        }
        public abstract string GetMyType();

        public static string PrintBoolean(bool value)
        {
            if (value) return "true";
            return "false";
        }
    }
}
