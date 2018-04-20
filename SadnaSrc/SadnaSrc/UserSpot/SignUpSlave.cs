using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using static System.Int32;

namespace SadnaSrc.UserSpot
{
    class SignUpSlave
    {
        private readonly UserServiceDL userDB;

        private readonly User _guest;

        public UserAnswer Answer { get; private set; }

        public SignUpSlave(User guest)
        {
            userDB = UserServiceDL.Instance;
            Answer = null;
            _guest = guest;
        }

        public User SignUp(string name, string address, string password, string creditCard)
        {
            try
            {
                ApproveSignUp(name, address, password, creditCard);
                MarketLog.Log("UserSpot", "User " + _guest.SystemID + " attempting to sign up to the system...");
                string encryptedPassword = UserSecurityService.ToEncryptPassword(_guest.SystemID,password);
                MarketLog.Log("UserSpot", "Searching for existing user and storing newly Registered User "
                                          + _guest.SystemID + " data...");
                RegisteredUser newRegistered = userDB.RegisterUser(_guest.SystemID,name, address, encryptedPassword,
                    creditCard, _guest.Cart.GetCartStorage());
                MarketLog.Log("UserSpot", "User " + newRegistered.SystemID + " sign up to the system has been successfull!");
                Answer = new UserAnswer(SignInStatus.Success, "Sign up has been successfull!");
                return newRegistered;
            }
            catch (UserException e)
            {
                MarketLog.Log("UserSpot", "User " + _guest.SystemID + " has failed to sign up. Error message has been created!");
                Answer = new UserAnswer((SignUpStatus)e.Status, e.GetErrorMessage());
                return _guest;
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
