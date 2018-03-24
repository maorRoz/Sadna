using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    class UserPolicyService
    {
        private List<UserPolicy> policies;
        private int _systemID;
        public List<UserPolicy> Policies
        {
            get{ return policies;}
        }

        public UserPolicyService(int systemID)
        {
            policies = new List<UserPolicy>();
            _systemID = systemID;
        }

        public void AddStorePolicy(string store, StoreAdminPolicy.StoreAction storeAction)
        {
            StoreAdminPolicy toAdd = new StoreAdminPolicy(store,storeAction); 
            policies.Add(toAdd);
            // add in db
        }

        public void UpdateStorePolicies(string store,List<StoreAdminPolicy.StoreAction> actionsToAdd)
        {
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
                // remove from db
            }
        }

        public void AddStatePolicy(UserPolicy.State state)
        {
            if (state == UserPolicy.State.StoreAdmin)
            {
                throw new UserException(
                    "Cannot add UserPolicy of a Store Manager role without describing the nature of the permission or the related store");
            }
            // save in db
        }

    }
}
