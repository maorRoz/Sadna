using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketData;

namespace SadnaSrc.UserSpot
{
    public class ViewCartSlave
    {

        private readonly User _user;

        public UserAnswer Answer { get; private set; }

        private int userID;
        public ViewCartSlave(User user)
        {
            Answer = null;
            _user = user;
            userID = user?.SystemID ?? -1;
        }
        public void ViewCart()
        {
            MarketLog.Log("UserSpot", "User " + userID + " attempting to view his cart...");
            try
            {
                ApproveEnetered();
                MarketLog.Log("UserSpot", "User " + userID + " has successfully retrieved his cart info...");
                Answer = new UserAnswer(ViewCartStatus.Success, "View of the user's cart has been granted successfully!",
                    _user.Cart.GetCartStorageToString());
            }
            catch (UserException e)
            {
                MarketLog.Log("UserSpot", "User " + userID + " has failed to View Cart. Error message has been created!");
                Answer = new UserAnswer((ViewCartStatus)e.Status, e.GetErrorMessage(), null);
            }
            catch (DataException e)
            {
                Answer = new UserAnswer((ViewCartStatus)e.Status, e.GetErrorMessage(),null);
            }

        }

        private void ApproveEnetered()
        {
            if (_user != null) { return; }
            throw new UserException(ViewCartStatus.DidntEnterSystem,
                "View Cart action has been requested by User which hasn't fully entered the system yet!");

        }
    }
}
