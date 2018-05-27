using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace SadnaSrc.MarketHarmony
{
    public class UserSellerHarmony : IUserSeller
    {
        private IUserService _user;
        private StoreManagerPolicy[] policies;
        private bool isSystemAdmin;
        private readonly string _store;
        private string managerName;
        private int managerID;

        public UserSellerHarmony(ref IUserService userService, string store)
        {
            _user = userService;
            _store = store;
            policies = null;
            isSystemAdmin = false;
            managerID = -1;
            GetUserDetails();
        }

        private void GetUserDetails()
        {
            var user = ((UserService)_user).MarketUser;
            if (user == null) { return; }

            managerID = user.SystemID;
            policies = user.GetStoreManagerPolicies(_store);
            isSystemAdmin = user.IsSystemAdmin();
            if (user.IsRegisteredUser())
            {
                managerName = ((RegisteredUser)user).Name;
            }
        }

        private void ValidateCanManageStore()
        {
            GetUserDetails();
            if(policies == null || (policies.Length == 0 && !isSystemAdmin))
            {
                throw new UserException(PromoteStoreStatus.NoAuthority,"Cannot manage store "+ _store +"!!");
            }
        }

        private void CanDoAction(StoreManagerPolicy.StoreAction action)
        {
            ValidateCanManageStore();
            if (CanDoAnything())
            {
                return;
            }
            ValidateHasPermission(action);
        }

        public void ValidateNotPromotingHimself(string userName)
        {
            if (managerName.Equals(userName))
            {
                throw new UserException(PromoteStoreStatus.PromoteSelf,"Cannot grant permissions to yourself!");
            }
        }

        public int GetID()
        {
            return managerID;
        }

        public string[] Promote(string userName, string permissions)
        {
            ValidateCanManageStore();
            ValidateNotPromotingHimself(userName);
            List<StoreManagerPolicy.StoreAction> actions = new List<StoreManagerPolicy.StoreAction>();
            var permissionsArray = permissions.Split(',').Distinct().ToArray();

            foreach (var permission in permissionsArray)
            {
                if (permission.Length == 0)
                {
                    continue;

                }
                StoreManagerPolicy.StoreAction action = StoreManagerPolicy.GetActionFromString(permission);
                if (!CanDoAnything() && !SearchPermission(action))
                {
                    throw new UserException(PromoteStoreStatus.PromotionOutOfReach, "Cannot grant permission " + permission+ " to others if you don't have it yourself");
                }
                actions.Add(action);
            }
           return PromoteStorePolicies(userName, actions.ToArray());
        }

        private string[] PromoteStorePolicies(string userName, StoreManagerPolicy.StoreAction[] permissions)
        {
            return UserPolicyService.PromoteStorePolicies(userName, _store, permissions);
        }

        private bool SearchPermission(StoreManagerPolicy.StoreAction permission)
        {
            foreach (StoreManagerPolicy policy in policies)
            {
                if (policy.Action == permission)
                {
                    return true;
                }
            }

            return false;
        }
        private void ValidateHasPermission(StoreManagerPolicy.StoreAction action)
        {
            if (!SearchPermission(action))
            {

                throw new UserException(PromoteStoreStatus.NoAuthority,
                    "Cannot do the required action in store " + _store + "!");
            }
        }

        private bool CanDoAnything()
        {
            return isSystemAdmin || SearchPermission(StoreManagerPolicy.StoreAction.StoreOwner);
        }

        public void CanPromoteStoreOwner()
        {
            CanDoAction(StoreManagerPolicy.StoreAction.StoreOwner);
        }

        public void CanPromoteStoreAdmin()
        {
            CanDoAction(StoreManagerPolicy.StoreAction.PromoteStoreAdmin);
        }

        public void CanManageProducts()
        {
            CanDoAction(StoreManagerPolicy.StoreAction.ManageProducts);
        }

        public void CanDeclarePurchasePolicy()
        {
            CanDoAction(StoreManagerPolicy.StoreAction.DeclarePurchasePolicy);
        }

        public void CanDeclareDiscountPolicy()
        {
            CanDoAction(StoreManagerPolicy.StoreAction.DeclareDiscountPolicy);
        }

        public void CanViewPurchaseHistory()
        {
            CanDoAction(StoreManagerPolicy.StoreAction.ViewPurchaseHistory);
        }

        public string GetName()
        {
            return managerName;
        }
    }
}
