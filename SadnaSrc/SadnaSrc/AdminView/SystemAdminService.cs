using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using SadnaSrc.UserSpot;

namespace SadnaSrc.AdminView
{
    public class SystemAdminService : ISystemAdminService
    {
        private int systemID;
        private bool _isSystemAdmin;
        private SystemAdminServiceDL adminDL;
        public PurchaseHistory[] LastHistoryReport { get; private set; }
        public SystemAdminService(UserService userService)
        {
            adminDL = new SystemAdminServiceDL();
            GetSystemAdmin(userService);
            LastHistoryReport = null;
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
                    "view purchase history action has been requested by User which hasn't fully identified as" +
                    " System Admin!");
            }
        }

        private void ApproveNotSelfTermination(int userSystemID)
        {
            if (userSystemID == systemID)
            { 
                throw new AdminException(RemoveUserStatus.SelfTermination, "remove user action has been requested " +
                                                                           "by System Admin on himself!");
            }
        }

        private void RemoveSolelyOwnedStores(int userSystemID)
        {
            string[] solelyOwnedStores = adminDL.FindSolelyOwnedStores();
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
            MarketLog.Log("AdminView", "System Admin " + systemID +
                                      " attempting to execute remove user operation on User " + userSystemID + "...");
            try
            {
                ApproveSystemAdmin("Remove User");
                ApproveNotSelfTermination(userSystemID);
                adminDL.IsUserExist(userSystemID);
                MarketLog.Log("AdminView", "User " + userSystemID +
                                           " has been found by the system. Removing user's saved cart and profile...");

                adminDL.DeleteUser(userSystemID);
                MarketLog.Log("AdminView", "System Admin " + systemID +
                                           " successfully removed User " + userSystemID + " from the system!");

                MarketLog.Log("AdminView", "looking for sole ownership of User "+userSystemID +" on stores...");
                RemoveSolelyOwnedStores(userSystemID);

                MarketLog.Log("AdminView", "User " + userSystemID +
                                           " solely owned stores has been deactivated. Operation is " +
                                           "finally completed safely!");
                return new AdminAnswer(RemoveUserStatus.Success, "Remove user has been successful!");
            }
            catch (AdminException e)
            {
                MarketLog.Log("AdminView", "System Admin " + systemID + " has failed to remove User "+ userSystemID + 
                                           ". Error message has been created!");
                return new AdminAnswer((RemoveUserStatus)e.Status, e.GetErrorMessage());
            }
        }

        private MarketAnswer ViewPurchaseHistory(string field, string givenValue)
        {
            MarketLog.Log("AdminView", "System Admin " + systemID +
                                       " attempting to view purchase history of " + field + " "+ givenValue + "...");
            try
            {
                LastHistoryReport = adminDL.GetPurchaseHistory(field, givenValue);
                return new AdminAnswer(ViewPurchaseHistoryStatus.Success, "View purchase history has been successful!");
            }
            catch (AdminException e)
            {
                MarketLog.Log("AdminView", "System Admin " + systemID + " has failed to view purchase history report " +
                                           "of " + field + " " + givenValue + ". Error message has been created!");
                return new AdminAnswer((ViewPurchaseHistoryStatus)e.Status, e.GetErrorMessage());
            }

        }
        public MarketAnswer ViewPurchaseHistoryByUser(string userName)
        {
            MarketLog.Log("AdminView", "System Admin " + systemID +
                                       " attempting to view purchase history of User " + userName + " ...");
            try
            {
                ApproveSystemAdmin("View Purchase History");
                adminDL.IsUserNameExistInHistory(userName);
                return ViewPurchaseHistory("UserName", userName);
            }
            catch (AdminException e)
            {
                MarketLog.Log("AdminView", "System Admin " + systemID + " has failed to view purchase history report " +
                                           "of User "+ userName + " . Error message has been created!");
                return new AdminAnswer((ViewPurchaseHistoryStatus)e.Status, e.GetErrorMessage());
            }
        }

        public MarketAnswer ViewPurchaseHistoryByStore(string storeName)
        {
            MarketLog.Log("AdminView", "System Admin " + systemID +
                                       " attempting to view purchase history of Store " + storeName + " ...");
            try
            {
                ApproveSystemAdmin("View Purchase History");
                adminDL.IsStoreExistInHistory(storeName);
                return ViewPurchaseHistory("Store",storeName);

            }
            catch (AdminException e)
            {
                MarketLog.Log("AdminView", "System Admin " + systemID + " has failed to view purchase history report " +
                                           "of Store " + storeName + " . Error message has been created!");
                return new AdminAnswer((ViewPurchaseHistoryStatus)e.Status, e.GetErrorMessage());
            }
        }
    }
}
