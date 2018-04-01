using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    public class UserService : IUserService
    {
        private User user;
        private readonly UserServiceDL userDL;
        private int systemID;
        private int oldID;

        public UserService()
        {
            userDL = new UserServiceDL();
            user = null;
            systemID = userDL.GetSystemID();
            oldID = systemID;
            Synch();
        }

        public void Synch()
        {
            UserException.SetUser(systemID);
            UserPolicyService.EstablishServiceDL(userDL);
            CartService.EstablishServiceDL(userDL);
        }
        public MarketAnswer EnterSystem()
        {
            MarketLog.Log("UserSpot", "User " + systemID + " attempting to enter the system...");
            user = new User(systemID);
            MarketLog.Log("UserSpot","User "+systemID+" has entered the system!");
            return new UserAnswer(EnterSystemStatus.Success,"You've been entered the system successfully!");
        }

        public User GetUser()
        {
            return user;
        }

        private void ApproveEnetered(string action)
        {
            if (user != null) return;
            if (action.Equals("sign up"))
            {
                throw new UserException(SignUpStatus.DidntEnterSystem,
                    "sign up action has been requested by User which hasn't fully entered the system yet!");
            }
            throw new UserException(SignInStatus.DidntEnterSystem,
                "sign in action has been requested by User which hasn't fully entered the system yet!");
        }

        private void ApproveGuest(string action)
        {
            if (user.GetStoreManagerPolicies().Length == 0 
                && !user.IsRegisteredUser())
            {
                return;
            }
            if (action.Equals("sign up"))
            {
                throw new UserException(SignUpStatus.SignedUpAlready,
                    "sign up action has been requested by registered user!");
            }
            throw new UserException(SignInStatus.SignedInAlready,
                "sign in action has been requested by registered user!");
        }

        private void ApproveSignUp(string name, string address, string password)
        {
            ApproveEnetered("sign up");
            ApproveGuest("sign up");
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(password))
            {
                throw new UserException(SignUpStatus.NullEmptyDataGiven,
                    "sign up action has been requested while some required fields are still missing!");
            }
        }
        private string ToEncryptPassword(string password)
        {
            MarketLog.Log("UserSpot", "encrypting User " + systemID + " password for security measures...");
            string encryptedPassword = GetSecuredPassword(password);
            MarketLog.Log("UserSpot", "User " + systemID + " password has been encrypted successfully!");
            return encryptedPassword;
        }

        public static string GetSecuredPassword(string password)
        {
            var secuirtyService = System.Security.Cryptography.MD5.Create();
            byte[] bytes = Encoding.Default.GetBytes(password);
            byte[] encodedBytes = secuirtyService.ComputeHash(bytes);

            StringBuilder newPasswordString = new StringBuilder();
            for (int i = 0; i < encodedBytes.Length; i++)
                newPasswordString.Append(encodedBytes[i].ToString("x2"));

            return newPasswordString.ToString();
        }
        public MarketAnswer SignUp(string name, string address, string password)
        {
            MarketLog.Log("UserSpot", "User " + systemID + " attempting to sign up to the system...");
            try
            {
                ApproveSignUp(name,address,password);
                string encryptedPassword = ToEncryptPassword(password);
                MarketLog.Log("UserSpot", "Searching for existing user and storing newly Registered User " + systemID + " data...");
                user = userDL.RegisterUser(name, address, encryptedPassword,user.GetCart());
                MarketLog.Log("UserSpot", "User " + systemID + " sign up to the system has been successfull!");
                return new UserAnswer(SignInStatus.Success,"Sign up has been successfull!");
            }
            catch (UserException e)
            {
                MarketLog.Log("UserSpot","User "+ systemID +" has failed to sign up. Error message has been created!");
                return new UserAnswer((SignUpStatus)e.Status,e.GetErrorMessage());
            }
        }

        private void ApproveSignIn(string name, string password)
        {
            ApproveEnetered("sign in");
            ApproveGuest("sign in");
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password))
            {
                throw new UserException(SignInStatus.NullEmptyDataGiven,
                    "sign in action has been requested while some required fields are still missing!");
            }
        }

        public MarketAnswer SignIn(string name, string password)
        {
            MarketLog.Log("UserSpot", "User " + systemID + " attempting to sign in the system...");
            try
            {
                ApproveSignIn(name, password);
                string encryptedPassword = ToEncryptPassword(password);
                MarketLog.Log("UserSpot", "Searching for existing user and logging in Guest " 
                                          + systemID +" into the system...");
                user = userDL.LoadUser(name, encryptedPassword,user.GetCart());
                systemID = user.SystemID;
                MarketLog.Log("UserSpot", "User " + oldID + " sign in to the system has been successfull!");
                MarketLog.Log("UserSpot", "User " + oldID + " is now recognized as Registered User " + systemID);
                return new UserAnswer(SignInStatus.Success, "Sign in has been successful!");

            }
            catch (UserException e)
            {
                MarketLog.Log("UserSpot", "User " + systemID + " has failed to sign in. Error message has been created!");
                return new UserAnswer((SignInStatus)e.Status, e.GetErrorMessage());
            }
        }

        public void CleanGuestSession()
        {
            Synch();
            userDL.DeleteUser(oldID);
        }

        public void CleanSession()
        {
            CleanGuestSession();
            userDL.DeleteUser(systemID);
        }
    }
}
