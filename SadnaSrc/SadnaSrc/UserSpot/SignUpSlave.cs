using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using static System.Int32;

namespace SadnaSrc.UserSpot
{
    public class SignUpSlave
    {
        private readonly IUserDL _userDB;

        private readonly User _guest;

        public UserAnswer Answer { get; private set; }

        private int currentID;

        public SignUpSlave(User guest, IUserDL userDB)
        {
            _userDB = userDB;
            Answer = null;
            _guest = guest;
            currentID = _guest?.SystemID ?? -1;
        }

        public User SignUp(string name, string address, string password, string creditCard)
        {
            MarketLog.Log("UserSpot", "User " + currentID + " attempting to sign up to the system...");
            try
            {
                ApproveSignUp(name, address, password, creditCard);
                string encryptedPassword = UserSecurityService.ToEncryptPassword(_guest.SystemID,password);
                MarketLog.Log("UserSpot", "Searching for existing user and storing newly Registered User "
                                          + currentID + " data...");
                ValidateUserNotExist(name);
                RegisteredUser newRegistered = _userDB.RegisterUser(_guest.SystemID,name, address, encryptedPassword,
                    creditCard, _guest.Cart.GetCartStorage());
                MarketLog.Log("UserSpot", "User " + newRegistered.SystemID + " sign up to the system has been successfull!");
                Answer = new UserAnswer(SignInStatus.Success, "Sign up has been successfull!");
                return newRegistered;
            }
            catch (UserException e)
            {
                MarketLog.Log("UserSpot", "User " + currentID + " has failed to sign up. Error message has been created!");
                Answer = new UserAnswer((SignUpStatus)e.Status, e.GetErrorMessage());
                return _guest;
            }
        }

        private void ValidateUserNotExist(string userName)
        {
            if (_userDB.IsUserNameExist(userName))
            {
                throw new UserException(SignUpStatus.TakenName, currentID + " register action has been requested while " +
                                                "there is already a User with the given name '"+userName+"' in the system!");
            }
        }

        private void ApproveEnetered()
        {
            if (_guest != null)
            {
                return;
            }
            throw new UserException(SignUpStatus.DidntEnterSystem,
                "sign up action has been requested by User which hasn't fully entered the system yet!");

        }

        private void ApproveGuest()
        {
            ApproveEnetered();
            if (!_guest.IsRegisteredUser())
            {
                return;
            }
            throw new UserException(SignUpStatus.SignedUpAlready,
               "sign up action has been requested by registered user!");
        }

        private static bool IsValidCreditCard(string creditCard)
        {
            int _;
            return !string.IsNullOrEmpty(creditCard) && creditCard.Length == 8 && TryParse(creditCard, out _);
        }

        private void ApproveSignUp(string name, string address, string password, string creditCard)
        {
            ApproveGuest();
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(password) || !IsValidCreditCard(creditCard))
            {
                throw new UserException(SignUpStatus.NullEmptyFewDataGiven,
                    "sign up action has been requested while some required fields are still missing or invalid!");
            }
        }


    }
}
