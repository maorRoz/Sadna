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

        private void ApproveSignUp(string name, string address, string password)
        {
            if (user == null)
            {
                throw new UserException(
                    "sign up action has been requested by user which hasn't fully entered the system yet!");
            }

            if (user.GetPolicies().Length > 0)
            {
                throw new UserException("sign up action has been requested by registered user!");
            }

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
                MarketLog.Log("UserSpot", "encrypting User " + systemID + " password for security measures...");
                string encryptedPassword = GetSecuredPassword(password);
                MarketLog.Log("UserSpot", "User " + systemID + " password has been encrypted successfully!");
                MarketLog.Log("UserSpot", "Searching for existing user and storing newly Registered User " + systemID + " data...");
                userDL.RegisterUser(name, address, encryptedPassword);
                user = new RegisteredUser(systemID, name, address, encryptedPassword);
                MarketLog.Log("UserSpot", "User " + systemID + " sign up to the system has been successfull!");
                return "Sign up has been successfull!";
            }
            catch (UserException e)
            {
                MarketLog.Log("UserSpot","User "+ systemID +" has failed to sign up. Error message has been created!");
                return e.GetErrorMessage();
            }
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
