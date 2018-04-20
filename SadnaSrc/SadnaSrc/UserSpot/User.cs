using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    public class User
    {
        protected readonly UserPolicyService PolicyService;

        public CartService Cart { get; }

        public int SystemID { get; }
        public User(int systemID)
        {
            SystemID = systemID;
            PolicyService = new UserPolicyService(SystemID);
            Cart = new CartService(SystemID);
        }

        public bool IsRegisteredUser()
        {
            return PolicyService.StatesPolicies.Count > 0 &&
                   PolicyService.StatesPolicies.Count < 3 &&
                   PolicyService.StatesPolicies.ElementAt(0).GetState() ==
                   StatePolicy.State.RegisteredUser;
        }

        public bool IsSystemAdmin()
        {
            return IsRegisteredUser() && PolicyService.StatesPolicies.Count == 2
                   && PolicyService.StatesPolicies.ElementAt(1).GetState() ==
                   StatePolicy.State.SystemAdmin;

        }

        public bool HasStorePolicies()
        {
            return PolicyService.StorePolicies.Count > 0;
        }

        public StoreManagerPolicy[] GetStoreManagerPolicies(string store)
        {
            return PolicyService.FilteredStorePolicies(store);
        }

        public virtual object[] ToData()
        {
            object[] ret = {SystemID, null, null, null,null};
            return ret;
        }
    }
}
