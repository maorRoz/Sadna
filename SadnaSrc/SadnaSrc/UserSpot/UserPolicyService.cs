﻿using System;
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
        public static void PromoteStorePolicies(string userName,string store,StoreManagerPolicy.StoreAction[] actionsToAdd)
        {
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
