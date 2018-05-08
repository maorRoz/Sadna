using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public interface IPolicyChecker
    {
        bool CheckRelevantPolicies(string product, string store, string category, string username, string address, int quantity, double price);
    }
}
