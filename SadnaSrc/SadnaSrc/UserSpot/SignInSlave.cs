using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    class SignInSlave
    {
        private readonly UserServiceDL userDB;

        private readonly User _guest;

        public UserAnswer Answer { get; private set; }

        public SignInSlave(User guest)
        {
            userDB = UserServiceDL.Instance;
            Answer = null;
            _guest = guest;
        }

        public User SignIn(string name, string password)
        {
            MarketLog.Log("UserSpot", "User " + _guest.SystemID + " attempting to sign in the system...");
            try
            {
                ApproveSignIn(name, password);
                string encryptedPassword = UserSecurityService.ToEncryptPassword(_guest.SystemID,password);
                MarketLog.Log("UserSpot", "Searching for existing user and logging in Guest "
                                          + _guest.SystemID + " into the system...");
                User loggedUser = userDB.LoadUser(name, encryptedPassword, _guest.Cart.GetCartStorage());
                MarketLog.Log("UserSpot", "User " + _guest.SystemID + " sign in to the system has been successfull!");
                MarketLog.Log("UserSpot", "User " + _guest.SystemID + " is now recognized as Registered User " + loggedUser.SystemID);
                Answer = new UserAnswer(SignInStatus.Success, "Sign in has been successful!");
                return loggedUser;

            }
            catch (UserException e)
            {
                MarketLog.Log("UserSpot",
                    "User " + _guest.SystemID + " has failed to sign in. Error message has been created!");
                Answer = new UserAnswer((SignInStatus) e.Status, e.GetErrorMessage());
                return _guest;
            }
        }
        private void ApproveEnetered()
        {
            if (_guest != null) { return; }
            throw new UserException(SignInStatus.DidntEnterSystem,
                "sign in action has been requested by User which hasn't fully entered the system yet!");

        }

        private void ApproveGuest()
        {
            ApproveEnetered();
            if (!_guest.IsRegisteredUser())
            {
                return;
            }
            throw new UserException(SignInStatus.SignedInAlready,
                "sign in action has been requested by registered user!");
        }

        private void ApproveSignIn(string name, string password)
        {
            ApproveGuest();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
            {
                throw new UserException(SignInStatus.NullEmptyDataGiven,
                    "sign in action has been requested while some required fields are still missing!");
            }
        }
    }
}
