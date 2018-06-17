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
        private readonly IUserAdmin _admin;

        public AdminAnswer Answer { get; private set; }


        public AdminViewPurchaseHistorySlave(IAdminDL adminDB, IUserAdmin admin)
        {
            _adminDB = adminDB;
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
            catch (DataException e)
            {
                Answer = new AdminAnswer((ViewPurchaseHistoryStatus)e.Status, e.GetErrorMessage(), null);
            }
            catch (MarketException e)
            {
                Answer = new AdminAnswer(ViewPurchaseHistoryStatus.NotSystemAdmin, e.GetErrorMessage(), null);
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
	            string similarUser = FindSimilarUserNames(userName);
	            if (similarUser != "")
	            {
		            throw new AdminException(ViewPurchaseHistoryStatus.MistakeTipGiven, "Couldn't find any Store" + " with that name in history records, did you mean " + similarUser + "?");

	            }
				throw new AdminException(ViewPurchaseHistoryStatus.NoUserFound, "Couldn't find any User with " +
                                                                                "that name in history records");
            }

        }

        private void ValidateStoreNameExistInPurchaseHistory(string storeName)
        {
	        
	        if (!_adminDB.IsStoreExistInHistory(storeName))
	        {
		        string similarStore = FindSimilarStoreName(storeName);
		        if (similarStore != "")
		        {
			        throw new AdminException(ViewPurchaseHistoryStatus.MistakeTipGiven, "Couldn't find any Store" + " with that name in history records, did you mean "+ similarStore+"?");

				}
				throw new AdminException(ViewPurchaseHistoryStatus.NoStoreFound, "Couldn't find any Store" + " with that name in history records");
			}

	       
            }

	    private string FindSimilarStoreName(string storeName)
	    {
		    string similarStore = "";
		    string[] storeNames = _adminDB.GetAllStoresInPurchaseHistory();
		    for (int i = 0; i < storeNames.Length; i++)
		    {
			    if (MarketMistakeService.IsSimilar(storeNames[i], storeName))
			    {
				    similarStore = storeNames[i];
				    break;

			    }
		    }
		    return similarStore;
	    }

	    private string FindSimilarUserNames(string userName)
	    {
		    string similarUser = "";
		    string[] userNames = _adminDB.GetAllUserInPurchaseHistory();
		    for (int i = 0; i < userNames.Length; i++)
		    {
			    if (MarketMistakeService.IsSimilar(userNames[i], userName))
			    {
				    similarUser = userNames[i];
				    break;

			    }
		    }
		    return similarUser;
	    }
	}
}
