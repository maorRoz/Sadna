using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.StoreCenter;

namespace SadnaSrc.UserSpot
{
    public class GetControlledStoreNamesSlave
    {
        private readonly User _user;
        public UserAnswer Answer { get; private set; }
	    private readonly IUserDL _userDB;

		private int userID;

        public GetControlledStoreNamesSlave(User user, IUserDL userDB)
        {
            Answer = null;
            _user = user;
	        _userDB = userDB;
			userID = user?.SystemID ?? -1;
        }

        public void GetControlledStoreNames()
        {
            try
            {
                MarketLog.Log("UserSpot", "User " + userID + " attempting to view which store he can manage...");
                ApproveEnetered();
                MarketLog.Log("UserSpot", "User " + userID + " has successfully retrieved all store names...");
	            Answer = new UserAnswer(GetControlledStoresStatus.Success, "View of store names has been granted successfully!",
	                FindControlledStoresOfUser());
				
            }
            catch (UserException e)
            {
                Answer = new UserAnswer((GetControlledStoresStatus)e.Status, e.GetErrorMessage());
            }
            catch (DataException e)
            {
                Answer = new UserAnswer((GetControlledStoresStatus)e.Status, e.GetErrorMessage());
            }
        }

        private string[] FindControlledStoresOfUser()
        {
            var allStores = _userDB.GetAllActiveStoreNames();
            if (_user.IsSystemAdmin())
            {
                return allStores;
            }
            var storesControlledByUser = _user.GetControlledStores();
            var activeStores = new List<string>();
            foreach (var userControlledStore in storesControlledByUser)
            {
                if (allStores.Contains(userControlledStore))
                {
                    activeStores.Add(userControlledStore);
                }
            }

            return activeStores.ToArray();
        }

	    public void ViewStores()
	    {
		    MarketLog.Log("UserSpot", "User " + userID + " attempting to view all store names...");
			try
		    {
			    ApproveEnetered();
				var storeNames = _userDB.GetAllActiveStoreNames();
			    Answer = new UserAnswer(ViewStoresStatus.Success, "you've got all the store names!", storeNames);
		    }
		    catch (UserException)
		    {
			    MarketLog.Log("UserSpot", "User " + userID + " has failed to view all store names." +
			                              " Error message has been created!");
				Answer = new UserAnswer(ViewStoresStatus.NoPermission, "The operation didn't succeed!");
		    }
		    
		}

		private void ApproveEnetered()
        {
            if (_user != null) { return; }
            throw new UserException(GetControlledStoresStatus.DidntEnterSystem,
                "View controlled stores names has been requested by User which hasn't fully entered the system yet!");

        }
    }
}
