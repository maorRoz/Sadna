using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.AdminView
{
    public class SystemAdminService : ISystemAdminService
    {
        private int adminSystemID;
        private IUserAdmin _admin;
        private string adminUserName;
        private bool _isSystemAdmin;
        private SystemAdminServiceDL adminDL;
        public SystemAdminService(IUserAdmin admin)
        {
            adminDL = new SystemAdminServiceDL();
            _admin = admin; 
            adminSystemID = _admin.GetAdminSystemID();
            adminUserName = _admin.GetAdminName();

        }



        private void ApproveNotSelfTermination(string userName)
        {
            if (adminUserName == userName)
            { 
                throw new AdminException(RemoveUserStatus.SelfTermination, "remove user action has been requested " +
                                                                           "by System Admin on himself!");
            }
        }

        private void RemoveSolelyOwnedStores(string userName)
        {
            string[] solelyOwnedStores = adminDL.FindSolelyOwnedStores();
            foreach (string store in solelyOwnedStores)
            {
                MarketLog.Log("AdminView", "User " + userName + " found to be a sole Store Owner of '"
                                           + store + "' store. System Admin " + adminSystemID
                                           + " deactivating therefore store '" + store + "'");
                adminDL.CloseStore(store);
                MarketLog.Log("AdminView", "System Admin " + adminSystemID +
                                           " deactivated store '" + store + "' successfully!");
            }
        }

        public MarketAnswer RemoveUser(string userName)
        {
            MarketLog.Log("AdminView", "System Admin " + adminSystemID +
                                      " attempting to execute remove user operation on User " + userName + "...");
            try
            {
                _admin.ValidateSystemAdmin();
                ApproveNotSelfTermination(userName);
                adminDL.IsUserExist(userName);
                MarketLog.Log("AdminView", "User " + userName +
                                           " has been found by the system. Removing user's saved cart and profile...");

                adminDL.DeleteUser(userName);
                MarketLog.Log("AdminView", "System Admin " + adminSystemID +
                                           " successfully removed User " + userName + " from the system!");

                MarketLog.Log("AdminView", "looking for sole ownership of User " + userName + " on stores...");
                RemoveSolelyOwnedStores(userName);

                MarketLog.Log("AdminView", "User " + userName +
                                           " solely owned stores has been deactivated. Operation is " +
                                           "finally completed safely!");
                return new AdminAnswer(RemoveUserStatus.Success, "Remove user has been successful!");
            }
            catch (AdminException e)
            {
                MarketLog.Log("AdminView", "System Admin " + adminSystemID + " has failed to remove User " +
                                           adminSystemID +
                                           ". Error message has been created!");
                return new AdminAnswer((RemoveUserStatus) e.Status, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                MarketLog.Log("AdminView", "User "+ adminSystemID + " tried to preform user removal not as system admin" +
                                           " and has been blocked. Error message has been created!");
                return new AdminAnswer(RemoveUserStatus.NotSystemAdmin, e.GetErrorMessage());
            }
        }

        private MarketAnswer ViewPurchaseHistory(string field, string givenValue)
        {
            MarketLog.Log("AdminView", "System Admin " + adminSystemID +
                                       " attempting to view purchase history of " + field + " "+ givenValue + "...");
            try
            {
                _admin.ValidateSystemAdmin();
                var historyReport = adminDL.GetPurchaseHistory(field, givenValue);
                return new AdminAnswer(ViewPurchaseHistoryStatus.Success, "View purchase history has been successful!",historyReport);
            }
            catch (MarketException e)
            {
                MarketLog.Log("AdminView", "User " + adminSystemID + " has tried to view purchase history report of others not as a " +
                                           "system admin and has been blocked. Error message has been created!");
                return new AdminAnswer(ViewPurchaseHistoryStatus.NotSystemAdmin, e.GetErrorMessage(),null);
            }


        }
        public MarketAnswer ViewPurchaseHistoryByUser(string userName)
        {
            MarketLog.Log("AdminView", "System Admin " + adminSystemID +
                                       " attempting to view purchase history of User " + userName + " ...");
            try
            {
                adminDL.IsUserNameExistInHistory(userName);
                return ViewPurchaseHistory("UserName", userName);
            }
            catch (AdminException e)
            {
                MarketLog.Log("AdminView", "System Admin " + adminSystemID + " has failed to view purchase history report " +
                                           "of User "+ userName + " . Error message has been created!");
                return new AdminAnswer((ViewPurchaseHistoryStatus)e.Status, e.GetErrorMessage(),null);
            }
        }

        public MarketAnswer ViewPurchaseHistoryByStore(string storeName)
        {
            MarketLog.Log("AdminView", "System Admin " + adminSystemID +
                                       " attempting to view purchase history of Store " + storeName + " ...");
            try
            {
                adminDL.IsStoreExistInHistory(storeName);
                return ViewPurchaseHistory("Store",storeName);

            }
            catch (AdminException e)
            {
                MarketLog.Log("AdminView", "System Admin " + adminSystemID + " has failed to view purchase history report " +
                                           "of Store " + storeName + " . Error message has been created!");
                return new AdminAnswer((ViewPurchaseHistoryStatus)e.Status, e.GetErrorMessage(),null);
            }
        }
    }
}
