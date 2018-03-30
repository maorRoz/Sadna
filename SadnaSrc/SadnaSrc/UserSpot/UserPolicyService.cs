using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    public class UserPolicyService
    {
        private List<UserPolicy> policies;
        private static UserServiceDL _userDL;
        public List<UserPolicy> Policies
        {
            get{ return policies;}
        }

        public UserPolicyService()
        {
            policies = new List<UserPolicy>();
        }

        public static void EstablishServiceDL(UserServiceDL userDL)
        {
            _userDL = userDL;
        }

        public static StoreManagerPolicy[] FilterStoreManagerPolicies(UserPolicy[] unFilteredPolicies)
        {
            List<StoreManagerPolicy> filteredStoreManagerPolicies = new List<StoreManagerPolicy>();

            foreach (UserPolicy policy in unFilteredPolicies)
            {
                if (policy.GetState() == UserPolicy.State.StoreManager)
                {
                    filteredStoreManagerPolicies.Add((StoreManagerPolicy)policy);
                }
            }

            return filteredStoreManagerPolicies.ToArray();

        }

        public void AddStorePolicy(string store, StoreManagerPolicy.StoreAction storeAction)
        {
            StoreManagerPolicy toAdd = new StoreManagerPolicy(storeAction, store); 
            policies.Add(toAdd);
            _userDL.SaveUserPolicy(toAdd);
        }

        public void UpdateStorePolicies(string store,StoreManagerPolicy.StoreAction[] actionsToAdd)
        {
            //TODO: check if store exist here or in DB query(with WHERE or something)
            foreach (StoreManagerPolicy.StoreAction oldAction in Enum.GetValues(typeof(StoreManagerPolicy.StoreAction)))
            {
                RemoveStorePolicy(store, oldAction);
            }
            foreach (StoreManagerPolicy.StoreAction action in actionsToAdd)
            {
                AddStorePolicy(store,action);
            }
            
        }
        private void RemoveStorePolicy(string store, StoreManagerPolicy.StoreAction storeAction)
        {
            StoreManagerPolicy toRemove = new StoreManagerPolicy(storeAction,store);
            if (policies.Remove(toRemove))
            {
                _userDL.DeleteUserPolicy(toRemove);
            }
        }

        public void AddStatePolicy(UserPolicy.State state)
        {
            if (state == UserPolicy.State.StoreManager)
            {
                throw new UserException(
                    MarketError.LogicError,"Cannot add UserPolicy of a Store Manager role without describing the nature of the permission or the related store");
            }
            _userDL.SaveUserPolicy(new UserPolicy(state));
            policies.Add(new UserPolicy(state));
        }

        public void LoadPolicies(UserPolicy[] loadedPolicies)
        {
            foreach (UserPolicy loadedPolicy in  loadedPolicies)
            {
                policies.Add(loadedPolicy);
            }
        }

    }
}
