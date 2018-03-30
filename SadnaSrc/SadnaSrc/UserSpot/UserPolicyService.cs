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

        public void AddStorePolicy(string store, StoreManagerPolicy.StoreAction storeAction)
        {
            StoreManagerPolicy toAdd = new StoreManagerPolicy(store, storeAction);
            StorePolicies.Add(toAdd);
            _userDL.SaveUserStorePolicy(toAdd);
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
            StoreManagerPolicy toRemove = new StoreManagerPolicy(store, storeAction);
            if (StorePolicies.Remove(toRemove))
            {
                _userDL.DeleteUserStorePolicy(toRemove);
            }
        }

        public void AddStatePolicy(StatePolicy.State state)
        {
            _userDL.SaveUserStatePolicy(new StatePolicy(state));
            StatesPolicies.Add(new StatePolicy(state));
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
