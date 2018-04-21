using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.AdminView
{
    class AdminViewPurchaseHistorySlave
    {
        private readonly IAdminDL _adminDB;
        private int adminSystemID;
        private IUserAdmin _admin;

        public AdminAnswer Answer { get; private set; }


        public AdminViewPurchaseHistorySlave(IAdminDL adminDB, IUserAdmin admin)
        {
            _adminDB = adminDB;
            Answer = null;
            _admin = admin;
            adminSystemID = _admin.GetAdminSystemID();
        }

        private void ViewPurchaseHistory(string field, string givenValue)
        {
            try
            {
                _admin.ValidateSystemAdmin();
                var historyReport = _adminDB.GetPurchaseHistory(field, givenValue);
                Answer = new AdminAnswer(ViewPurchaseHistoryStatus.Success, "View purchase history has been successful!", historyReport);
            }
            catch (MarketException e)
            {
                MarketLog.Log("AdminView", "User " + adminSystemID + " has tried to view purchase history report of others not as a " +
                                           "system admin and has been blocked. Error message has been created!");
                Answer = new AdminAnswer(ViewPurchaseHistoryStatus.NotSystemAdmin, e.GetErrorMessage(), null);
            }


        }
        public void ViewPurchaseHistoryByUser(string userName)
        {
            MarketLog.Log("AdminView", "System Admin " + adminSystemID +
                                       " attempting to view purchase history of User " + userName + " ...");
            try
            {
                ValidateUserNameExistInPurchaseHistory(userName);
                ViewPurchaseHistory("UserName", userName);
            }
            catch (AdminException e)
            {
                MarketLog.Log("AdminView", "System Admin " + adminSystemID + " has failed to view purchase history report " +
                                           "of User " + userName + " . Error message has been created!");
                Answer = new AdminAnswer((ViewPurchaseHistoryStatus)e.Status, e.GetErrorMessage(), null);
            }
        }

        public void ViewPurchaseHistoryByStore(string storeName)
        {
            MarketLog.Log("AdminView", "System Admin " + adminSystemID +
                                       " attempting to view purchase history of Store " + storeName + " ...");
            try
            {
                ValidateStoreNameExistInPurchaseHistory(storeName);
                ViewPurchaseHistory("Store", storeName);

            }
            catch (AdminException e)
            {
                MarketLog.Log("AdminView", "System Admin " + adminSystemID + " has failed to view purchase history report " +
                                           "of Store " + storeName + " . Error message has been created!");
                Answer = new AdminAnswer((ViewPurchaseHistoryStatus)e.Status, e.GetErrorMessage(), null);
            }
        }

        private void ValidateUserNameExistInPurchaseHistory(string userName)
        {
            if (!_adminDB.IsUserNameExistInHistory(userName))
            {
                throw new AdminException(ViewPurchaseHistoryStatus.NoUserFound, "Couldn't find any User with " +
                                                                                "that ID in history records");
            }

        }

        private void ValidateStoreNameExistInPurchaseHistory(string storeName)
        {
            if (!_adminDB.IsStoreExistInHistory(storeName))
            {
                throw new AdminException(ViewPurchaseHistoryStatus.NoStoreFound, "Couldn't find any Store" +
                                                                                 " with that name in history records");
            }
        }
    }
}
