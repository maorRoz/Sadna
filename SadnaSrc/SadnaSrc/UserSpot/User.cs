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
        protected int systemID;
        protected readonly CartService cart;

        public CartService Cart
        {
            get { return cart; }
        }

        public int SystemID
        {
            get { return systemID; }
        }
        public User(int systemID)
        {
            PolicyService = new UserPolicyService();
            cart = new CartService(systemID);
            this.systemID = systemID;
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

        public StoreManagerPolicy[] GetStoreManagerPolicies()
        {
            return PolicyService.StorePolicies.ToArray();
        }

        public CartItem[] GetCart()
        {
            return cart.GetCartStorage();
        }
        public virtual object[] ToData()
        {
            object[] ret = {systemID, null, null, null};
            return ret;
        }
    }
}
