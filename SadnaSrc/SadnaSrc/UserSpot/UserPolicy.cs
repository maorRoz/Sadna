using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    class UserPolicy
    {
        private Dictionary<string, string> storePolicies;
        private List<string> userPolicies;

        public bool addStorePolicy(string store, string details) { return false; }
        public bool removeStorePolicy(string store, string details) { return false; }
        public bool addSystemPolicy(string policyKind, string details) { return false; }

    }
}
