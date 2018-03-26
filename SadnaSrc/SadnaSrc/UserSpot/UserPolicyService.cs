using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void AddStorePolicy(string store, StoreAdminPolicy.StoreAction storeAction)
        {
            StoreAdminPolicy toAdd = new StoreAdminPolicy(store,storeAction); 
            policies.Add(toAdd);
            _userDL.SaveUserPolicy(toAdd);
        }

        public void UpdateStorePolicies(string store,List<StoreAdminPolicy.StoreAction> actionsToAdd)
        {
            //TODO: check if store exist here on in DB query(with WHERE or something)
            foreach (StoreAdminPolicy.StoreAction oldAction in Enum.GetValues(typeof(StoreAdminPolicy.StoreAction)))
            {
                RemoveStorePolicy(store, oldAction);
            }
            foreach (StoreAdminPolicy.StoreAction action in actionsToAdd)
            {
                AddStorePolicy(store,action);
            }
            
        }
        private void RemoveStorePolicy(string store, StoreAdminPolicy.StoreAction storeAction)
        {
            StoreAdminPolicy toRemove = new StoreAdminPolicy(store,storeAction);
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
                    "Cannot add UserPolicy of a Store Manager role without describing the nature of the permission or the related store");
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
