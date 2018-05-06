using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    interface IPolicyDL
    {
        void SavePolicy(PurchasePolicy policy);
        void RemovePolicy(PurchasePolicy policy);
        PurchasePolicy GetPolicy(PolicyType type, string subject);
        List<PurchasePolicy> GetPoliciesOfType(PolicyType type);
        List<PurchasePolicy> GetAllPolicies();
    }
}
