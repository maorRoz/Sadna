using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    public class GetControlledStoreNamesSlave
    {
        private readonly User _user;
        public UserAnswer Answer { get; private set; }

        private int userID;

        public GetControlledStoreNamesSlave(User user)
        {
            Answer = null;
            _user = user;
            userID = user?.SystemID ?? -1;
        }

        public void GetControlledStoreNames()
        {
            MarketLog.Log("UserSpot", "User " + userID + " attempting to view which store he can manage...");
            try
            {
                ApproveEnetered();
                MarketLog.Log("UserSpot", "User " + userID + " has successfully retrieved all store names...");
                Answer = new UserAnswer(GetControlledStoresStatus.Success, "View of store names has been granted successfully!",
                    _user.GetControlledStores());
            }
            catch (UserException e)
            {
                MarketLog.Log("UserSpot", "User " + userID + " has failed to View his controlled stores." +
                                          " Error message has been created!");
                Answer = new UserAnswer((GetControlledStoresStatus)e.Status, e.GetErrorMessage());
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
