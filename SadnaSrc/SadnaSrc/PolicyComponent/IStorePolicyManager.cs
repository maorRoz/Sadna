using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public interface IStorePolicyManager : IPolicyHandler
    {
        string[] CreateStoreSimplePolicy(string store, ConditionType cond, string value);
        string[] CreateStockItemSimplePolicy(string store, string product, ConditionType cond, string value);
        string[] CreateStorePolicy(string store, OperatorType op, int id1, int id2);
        string[] CreateStockItemPolicy(string store, string product, OperatorType op, int id1, int id2);
    }
}
