using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace SadnaSrc.AdminView
{
    class SystemAdminService : ISystemAdminService
    {
        private int systemID;
        private bool _isSystemAdmin;
        private SystemAdminServiceDL adminDL;
        public SystemAdminService(SQLiteConnection dbConnection, UserService userService)
        {
            adminDL = new SystemAdminServiceDL(dbConnection);
            GetSystemAdmin(userService);
        }

        private void GetSystemAdmin(UserService userService)
        {
            User user = userService.GetUser();
            _isSystemAdmin = hasEntered(user) && HasSystemAdminPolicy(user.GetStatePolicies());

            if (_isSystemAdmin)
            {
                systemID = user.SystemID;
            }
        }

        private bool hasEntered(User user)
        {
            return user != null;
        }
        private bool HasSystemAdminPolicy(StatePolicy[] policies)
        {
            if (policies.Length > 1)
            {
                foreach (StatePolicy policy in policies)
                {
                    if (policy.GetState() == StatePolicy.State.SystemAdmin)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void ApproveSystemAdmin(string action)
        {
            if (_isSystemAdmin)
            {
                return;
            }

            if (action.Equals("Remove User"))
            {

                throw new AdminException(RemoveUserStatus.NotSystemAdmin,
                    "remove user action has been requested by User which hasn't fully identified as System Admin!");
            }

            if (action.Equals("View Purchase History"))
            {
                throw new AdminException(ViewPurchaseHistoryStatus.NotSystemAdmin,
                    "view purchase history action has been requested by User which hasn't fully identified as System Admin!");
            }
        }

     /*   private void LookForSoleStoreOwners(int userSystemID, StoreManagerPolicy[] storeManagerPolicies)
        {
            foreach (StoreManagerPolicy policy in storeManagerPolicies)
            {
                if (policy.Action != StoreManagerPolicy.StoreAction.StoreOwner) { continue; }
                // look for more policies of store owner with that store name, if there is non, close the store.
                bool isSoleOwner = adminDL.GetOwners(policy.Store).Length > 1;
                if (!isSoleOwner) { continue; }
                MarketLog.Log("AdminView", "User " + userSystemID + " found to be a sole Store Owner of '"
                                          + policy.Store + "' store. System Admin " + systemID
                                          + " deactivating therefore store '" + policy.Store + "'");
                adminDL.CloseStore(policy.Store);
                MarketLog.Log("AdminView", "System Admin " + systemID +
                                          " deactivated store '" + policy.Store + "' successfully!");
            }
        }*/

        private void RemoveSolelyOwnedStores(int userSystemID)
        {
            string[] solelyOwnedStores = adminDL.FindSolelyOwnedStores(userSystemID);
            foreach (string store in solelyOwnedStores)
            {
                MarketLog.Log("AdminView", "User " + userSystemID + " found to be a sole Store Owner of '"
                                           + store + "' store. System Admin " + systemID
                                           + " deactivating therefore store '" + store + "'");
                adminDL.CloseStore(store);
                MarketLog.Log("AdminView", "System Admin " + systemID +
                                           " deactivated store '" + store + "' successfully!");
            }
        }
        public MarketAnswer RemoveUser(int userSystemID)
        {
            ApproveSystemAdmin("Remove User");
            MarketLog.Log("AdminView", "System Admin " + systemID +
                                      " attempting to execute remove user procedure on User " + userSystemID + "...");
            try
            {
                adminDL.IsUserExist(userSystemID);
                MarketLog.Log("AdminView", "User " + userSystemID +
                                           " has been found by the system. looking for sole ownership on stores...");

                //  UserPolicy[] toDeletePolicies = adminDL.LoadPolicies(userSystemID);
                //   StoreManagerPolicy[] storeManagerPolicies = adminDL.LoadStoreManagerPolicies(userSystemID);
                RemoveSolelyOwnedStores(userSystemID);
                // LookForSoleStoreOwners(userSystemID,storeManagerPolicies);

                MarketLog.Log("AdminView", "User " + userSystemID +
                                           " solely owned stores has been deactivated. Finally removing user saved cart and profile...");
                adminDL.DeleteUser(userSystemID);
                MarketLog.Log("AdminView", "System Admin " + systemID +
                                           " success fully removed User " + userSystemID + " from the system!");
                return new AdminAnswer(RemoveUserStatus.Success, "Remove user has been successful!");
            }
            catch (AdminException e)
            {
                MarketLog.Log("AdminView", "System Admin " + systemID + " has failed to remove User "+ userSystemID + 
                                           ". Error message has been created!");
                return new AdminAnswer((RemoveUserStatus)e.Status, e.GetErrorMessage());
            }
        }

        public MarketAnswer ViewPurchaseHistory(int userSystemID)
        {
            ApproveSystemAdmin("View Purchase History");
            return null;
        }

        public MarketAnswer ViewPurchaseHistory(string storeName)
        {
            ApproveSystemAdmin("View Purchase History");
            return null;
        }
    }
}
