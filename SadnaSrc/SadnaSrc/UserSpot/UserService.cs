using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    public class UserService : IUserService,ISystemAdminService
    {
        private User user;
        private readonly UserServiceDL userDL;
        private int systemID;

        public UserService(SQLiteConnection dbConnection)
        {
            userDL = new UserServiceDL(dbConnection);
            systemID = userDL.GetSystemID();
            ReConnect();
        }

        public void ReConnect()
        {
            UserException.SetUser(systemID);
            UserPolicyService.EstablishServiceDL(userDL);
            CartService.EstablishServiceDL(userDL);
        }
        public string EnterSystem()
        {
            MarketLog.Log("UserSpot", "User " + systemID + " attempting to enter the system...");
            user = new User(systemID);
            MarketLog.Log("UserSpot","User "+systemID+" has entered the system!");
            return "You've been entered the system successfully!";
        }

        public User GetUser()
        {
            return user;
        }

        private void ApproveEnetered(string action)
        {
            if (user == null)
            {
                throw new UserException(
                    action + " action has been requested by user which hasn't fully entered the system yet!");
            }
        }

        private void ApproveGuest(string action)
        {
            if (user == null)
            {
                throw new UserException(
                    action + " action has been requested by user which hasn't fully entered the system yet!");
            }

            if (user.GetPolicies().Length > 0)
            {
                throw new UserException(action + " action has been requested by registered user!");
            }
        }

        private void ApproveSignUp(string name, string address, string password)
        {
            ApproveEnetered("sign up");
            ApproveGuest("sign up");
            if (name == null || address == null || password == null)
            {
                throw new UserException(
                    "sign up action has been requested while some required fields are still missing!");
            }
        }
        public string SignUp(string name, string address, string password)
        {
            MarketLog.Log("UserSpot", "User " + systemID + " attempting to sign up to the system...");
            try
            {
                ApproveSignUp(name,address,password);
                string encryptedPassword = ToEncryptPassword(password);
                MarketLog.Log("UserSpot", "Searching for existing user and storing newly Registered User " + systemID + " data...");
                user = userDL.RegisterUser(name, address, encryptedPassword,user.GetCart());
                MarketLog.Log("UserSpot", "User " + systemID + " sign up to the system has been successfull!");
                return "Sign up has been successfull!";
            }
            catch (UserException e)
            {
                MarketLog.Log("UserSpot","User "+ systemID +" has failed to sign up. Error message has been created!");
                return e.GetErrorMessage();
            }
        }

        private void ApproveSignIn(string name, string password)
        {
            ApproveEnetered("sign in");
            ApproveGuest("sign in");
            if (name == null || password == null)
            {
                throw new UserException(
                    "sign up action has been requested while some required fields are still missing!");
            }
        }

        public string SignIn(string name, string password)
        {
            MarketLog.Log("UserSpot", "User " + systemID + " attempting to sign in the system...");
            try
            {
                ApproveSignIn(name, password);
                string encryptedPassword = ToEncryptPassword(password);
                MarketLog.Log("UserSpot", "Searching for existing user and logging in Guest " + systemID +" into the system...");
                user = userDL.LoadUser(name, encryptedPassword,user.GetCart()); //TODO implement LoadUser
                MarketLog.Log("UserSpot", "User " + systemID + " sign in to the system has been successfull!");
                return "Sign in has been successfull!";

            }
            catch (UserException e)
            {
                MarketLog.Log("UserSpot", "User " + systemID + " has failed to sign in. Error message has been created!");
                return e.GetErrorMessage();
            }
        }

        private string ToEncryptPassword(string password)
        {
            MarketLog.Log("UserSpot", "encrypting User " + systemID + " password for security measures...");
            string encryptedPassword = GetSecuredPassword(password);
            MarketLog.Log("UserSpot", "User " + systemID + " password has been encrypted successfully!");
            return encryptedPassword;
        }

        public string GetSecuredPassword(string password)
        {
            var secuirtyService = System.Security.Cryptography.MD5.Create();
            byte[] bytes = Encoding.Default.GetBytes(password);
            byte[] encodedBytes = secuirtyService.ComputeHash(bytes);

            StringBuilder newPasswordString = new StringBuilder();
            for (int i = 0; i < encodedBytes.Length; i++)
                newPasswordString.Append(encodedBytes[i].ToString("x2"));

            return newPasswordString.ToString();
        }


        // only for white box tests
        public void CleanSession()
        {
            userDL.DeleteUser();
        }
    }
}
