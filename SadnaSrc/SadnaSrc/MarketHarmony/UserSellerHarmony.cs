using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace SadnaSrc.MarketHarmony
{
    class UserSellerHarmony : IUserSeller
    {
        public StoreManagerPolicy[] policies;
        private string _store;
        public UserSellerHarmony(IUserService userServic,string store)
        {
            _store = store;
            policies = ((UserService) userServic).MarketUser.GetStoreManagerPolicies(store);
        }
        public void Promote(string userName, StoreManagerPolicy.StoreAction[] permissions)
        {
            UserPolicyService.PromoteStorePolicies(userName, _store, permissions);
        }

        private bool SearchPermission(StoreManagerPolicy.StoreAction action)
        {
            foreach (StoreManagerPolicy policy in policies)
            {
                if (policy.Action == action)
                {
                    return true;
                }
            }

            return false;
        }
        private bool IsStoreOwner()
        {
            return SearchPermission(StoreManagerPolicy.StoreAction.StoreOwner);
        }

        public bool CanPromoteStoreAdmin()
        {
            return IsStoreOwner() || SearchPermission(StoreManagerPolicy.StoreAction.PromoteStoreAdmin);
        }

        public bool CanManagePurchasePolicy()
        {
            return IsStoreOwner() || SearchPermission(StoreManagerPolicy.StoreAction.ManageProducts);
        }

        public bool CanDeclarePurchasePolicy()
        {
            return IsStoreOwner() || SearchPermission(StoreManagerPolicy.StoreAction.DeclarePurchasePolicy);
        }

        public bool CanDeclareDiscountPolicy()
        {
            return IsStoreOwner() || SearchPermission(StoreManagerPolicy.StoreAction.DeclareDiscountPolicy);
        }

        public bool CanViewPurchaseHistory()
        {
            return IsStoreOwner() || SearchPermission(StoreManagerPolicy.StoreAction.ViewPurchaseHistory);
        }
    }
}
