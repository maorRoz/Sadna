using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public class PolicyDL : IPolicyDL
    {
        public void SavePolicy(PurchasePolicy policy)
        {
            throw new NotImplementedException();
        }

        public void RemovePolicy(PurchasePolicy policy)
        {
            throw new NotImplementedException();
        }

        public PurchasePolicy GetPolicy(PolicyType type, string subject)
        {
            throw new NotImplementedException();
        }

        public List<PurchasePolicy> GetAllPolicies()
        {
            throw new NotImplementedException();
        }

        public List<PurchasePolicy> GetPoliciesOfType(PolicyType type)
        {
            throw new NotImplementedException();
        }
    }
}
