using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    class User
    {
        protected readonly UserPolicyService PolicyService;
        protected int systemID;
        public User(int systemID)
        {
            PolicyService = new UserPolicyService();
            this.systemID = systemID;
        }

        public UserPolicy[] GetPolicies()
        {
            return PolicyService.Policies.ToArray();
        }
    }
}
