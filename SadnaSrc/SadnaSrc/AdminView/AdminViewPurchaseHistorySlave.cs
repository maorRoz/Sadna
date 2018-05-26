using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

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
                Answer = new AdminAnswer(ViewPurchaseHistoryStatus.Success,
                    "View purchase history has been successful!", historyReport);
            }
            catch (MarketException e)
            {
                Answer = new AdminAnswer(ViewPurchaseHistoryStatus.NotSystemAdmin, e.GetErrorMessage(), null);
            }
            catch (DataException e)
            {
                Answer = new AdminAnswer((ViewPurchaseHistoryStatus)e.Status, e.GetErrorMessage(), null);
            }


        }
        public void ViewPurchaseHistoryByUser(string userName)
        {
            try
            {
                MarketLog.Log("AdminView", "System Admin " + adminSystemID +
                                           " attempting to view purchase history of User " + userName + " ...");
                ValidateUserNameExistInPurchaseHistory(userName);
                ViewPurchaseHistory("UserName", userName);
            }
            catch (AdminException e)
            {
                Answer = new AdminAnswer((ViewPurchaseHistoryStatus)e.Status, e.GetErrorMessage(), null);
            }
            catch (DataException e)
            {
                Answer = new AdminAnswer((ViewPurchaseHistoryStatus)e.Status, e.GetErrorMessage(), null);
            }
        }

        public void ViewPurchaseHistoryByStore(string storeName)
        {
            try
            {
                MarketLog.Log("AdminView", "System Admin " + adminSystemID +
                                           " attempting to view purchase history of Store " + storeName + " ...");
                ValidateStoreNameExistInPurchaseHistory(storeName);
                ViewPurchaseHistory("Store", storeName);

            }
            catch (AdminException e)
            {
                Answer = new AdminAnswer((ViewPurchaseHistoryStatus) e.Status, e.GetErrorMessage(), null);
            }
            catch (DataException e)
            {
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
    }
}
