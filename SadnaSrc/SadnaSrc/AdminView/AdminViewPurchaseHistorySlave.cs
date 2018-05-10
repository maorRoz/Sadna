using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;

namespace SadnaSrc.AdminView
{
    public class AdminViewPurchaseHistorySlave
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
                string realname = getRealUserName(userName);
                ValidateUserNameExistInPurchaseHistory(realname);
                ViewPurchaseHistory("UserName", realname);
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
                string realname = getRealStoreName(storeName);
                ValidateStoreNameExistInPurchaseHistory(realname);
                ViewPurchaseHistory("Store", realname);

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
                                                                                "that name in history records");
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
        private string getRealUserName(string username)
        {
            string[] allUserNames = _adminDB.GetAllUserNames();

            foreach (string user in allUserNames)
            {
                if (user == username)
                    return user;
            }
            foreach (string user in allUserNames)
            {
                if (MarketMistakeService.IsSimilar(user, username))
                    return user;
            }
            throw new AdminException(ViewPurchaseHistoryStatus.NoUserFound, "User Name Not Found");
        }
        private string getRealStoreName(string storename)
        {
            string[] allStoreNames = _adminDB.GetAllStoreNames();

            foreach (string store in allStoreNames)
            {
                if (store == storename)
                    return store;
            }
            foreach (string user in allStoreNames)
            {
                if (MarketMistakeService.IsSimilar(user, storename))
                    return user;
            }
            throw new AdminException(ViewPurchaseHistoryStatus.NoStoreFound, "Store Name Not Found");
        }
    }
}
