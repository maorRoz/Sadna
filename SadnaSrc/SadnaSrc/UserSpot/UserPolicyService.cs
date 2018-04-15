using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.UserSpot
{
    public class UserPolicyService
    {
        private static UserServiceDL _userDL;
        public List<StoreManagerPolicy> StorePolicies { get; }
        public List<StatePolicy> StatesPolicies { get; }

        public UserPolicyService()
        {
            StatesPolicies = new List<StatePolicy>();
            StorePolicies = new List<StoreManagerPolicy>();
        }

        public static void EstablishServiceDL(UserServiceDL userDL)
        {
            _userDL = userDL;
        }       
        public static void PromoteStorePolicies(string userName,string store,StoreManagerPolicy.StoreAction[] actionsToAdd)
        {
            if (!_userDL.IsUserNameExist(userName))
            {
                throw new UserException(PromoteStoreStatus.NoUserFound, "No user by the name '" + userName + " has been found for promotion!");
            }
            foreach (StoreManagerPolicy.StoreAction oldAction in Enum.GetValues(typeof(StoreManagerPolicy.StoreAction)))
            {
                _userDL.DeleteUserStorePolicy(userName, new StoreManagerPolicy(store, oldAction));
            }


            if (actionsToAdd.Contains(StoreManagerPolicy.StoreAction.StoreOwner))
            {
                actionsToAdd = new []{ StoreManagerPolicy.StoreAction.StoreOwner };
            }

            foreach (StoreManagerPolicy.StoreAction action in actionsToAdd)
            {
                _userDL.SaveUserStorePolicy(userName,new StoreManagerPolicy(store,action));
            }
            
        }

        public void AddStatePolicy(StatePolicy.State state)
        {
            _userDL.SaveUserStatePolicy(new StatePolicy(state));
            StatesPolicies.Add(new StatePolicy(state));
        }

        public StoreManagerPolicy[] FilteredStorePolicies(string store)
        {
            List<StoreManagerPolicy> storePolicies = new List<StoreManagerPolicy>();
            foreach (StoreManagerPolicy policy in StorePolicies)
            {
                if(policy.Store.Equals(store))
                storePolicies.Add(policy);
            }
            return SortStorePolicy(storePolicies.ToArray());
        }

        private static StoreManagerPolicy[] SortStorePolicy(StoreManagerPolicy[] storePolicies)
        {
            return storePolicies.OrderBy(x => x.Action).ToArray();
        }

        public void AddStoreOwnership(string store)
        {
            StoreManagerPolicy storeOwnershipPermission =
                new StoreManagerPolicy(store, StoreManagerPolicy.StoreAction.StoreOwner);
            StorePolicies.Add(storeOwnershipPermission);
            _userDL.SaveUserStorePolicy(storeOwnershipPermission);
        }

        public void LoadPolicies(StatePolicy[] loadedStates, StoreManagerPolicy[] loadedStorePermissions)
        {
            foreach (StatePolicy loadedState in loadedStates)
            {
                StatesPolicies.Add(loadedState);
            }

            foreach (StoreManagerPolicy loadedStorePermission in loadedStorePermissions)
            {
                StorePolicies.Add(loadedStorePermission);
            }
        }

    }
}
