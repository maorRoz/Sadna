using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    public static class UserSecurityService
    {
        public static string ToEncryptPassword(int userID,string password)
        {
            MarketLog.Log("UserSpot", "encrypting User " + userID + " password for security measures...");
            string encryptedPassword = GetSecuredPassword(password);
            MarketLog.Log("UserSpot", "User " + userID + " password has been encrypted successfully!");
            return encryptedPassword;
        }

        public static string GetSecuredPassword(string password)
        {
            var secuirtyService = System.Security.Cryptography.MD5.Create();
            var bytes = Encoding.Default.GetBytes(password);
            var encodedBytes = secuirtyService.ComputeHash(bytes);

            var newPasswordString = new StringBuilder();
            for (int i = 0; i < encodedBytes.Length; i++)
                newPasswordString.Append(encodedBytes[i].ToString("x2"));

            return newPasswordString.ToString();
        }
    }
}
