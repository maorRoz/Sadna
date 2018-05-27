using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public interface IGlobalPolicyManager : IPolicyHandler
    {
        string[] CreateGlobalSimplePolicy(ConditionType cond, string value);
        string[] CreateCategorySimplePolicy(string store, ConditionType cond, string value);
        string[] CreateProductSimplePolicy(string product, ConditionType cond, string value);
        string[] CreateGlobalPolicy(OperatorType op, int id1, int id2);
        string[] CreateCategoryPolicy(string category, OperatorType op, int id1, int id2);
        string[] CreateProductPolicy(string product, OperatorType op, int id1, int id2);
        string[] ViewPolicies();
        void CleanSession();
    }
}
