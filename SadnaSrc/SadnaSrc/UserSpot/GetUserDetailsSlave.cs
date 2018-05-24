using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketData;

namespace SadnaSrc.UserSpot
{
    public class GetUserDetailsSlave
    {
        private readonly User _user;
        public UserAnswer Answer { get; private set; }

        private int userID;

        public GetUserDetailsSlave(User user)
        {
            Answer = null;
            _user = user;
            userID = user?.SystemID ?? -1;
        }

        public void GetUserDetails()
        {
            MarketLog.Log("UserSpot", "User " + userID + " attempting to view his own details...");
            try
            {
                ApproveEnetered();
                var userDetails = ExtractDetails();
                MarketLog.Log("UserSpot", "User " + userID + " has successfully retrieved all his own user details...");

                Answer = new UserAnswer(GetControlledStoresStatus.Success, "Data of user details has been granted successfully!",
                    userDetails);
            }
            catch (UserException e)
            {
                MarketLog.Log("UserSpot", "User " + userID + " has failed to retrieve his user details." +
                                          " Error message has been created!");
                Answer = new UserAnswer((GetControlledStoresStatus)e.Status, e.GetErrorMessage());
            }
            catch (DataException e)
            {
                Answer = new UserAnswer((GetControlledStoresStatus)e.Status, e.GetErrorMessage());
            }
        }

        private void ApproveEnetered()
        {
            if (_user != null) { return; }
            throw new UserException(GetControlledStoresStatus.DidntEnterSystem,
                "Get User Details has been requested by User which hasn't fully entered the system yet!");

        }

        private string[] ExtractDetails()
        {
            var rawUserData = _user.ToData();
            var userDetails = new []
            {
                (string)rawUserData[1],
                (string)rawUserData[2],
                (string)rawUserData[4]
            };
            return userDetails;

        }
    }
}
