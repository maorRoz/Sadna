using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    class ViewCartSlave
    {
        private readonly UserServiceDL userDB;

        private readonly User _user;

        public UserAnswer Answer { get; private set; }
        public ViewCartSlave(User user)
        {
            userDB = UserServiceDL.Instance;
            Answer = null;
            _user = user;
        }
        public void ViewCart()
        {
            try
            {
                ApproveEnetered();
                MarketLog.Log("UserSpot", "User " + _user.SystemID + " attempting to view his cart...");

                MarketLog.Log("UserSpot", "User " + _user.SystemID + " has successfully retrieved his cart info...");
                Answer = new UserAnswer(ViewCartStatus.Success, "View of the user's cart has been granted successfully!",
                    _user.Cart.GetCartStorageToString());
            }
            catch (UserException e)
            {
                MarketLog.Log("UserSpot", "User " + _user.SystemID + " has failed to View Cart. Error message has been created!");
                Answer = new UserAnswer((ViewCartStatus)e.Status, e.GetErrorMessage(), null);
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
