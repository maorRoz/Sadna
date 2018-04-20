using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public enum MarketError
    {
        DbError,
        LogicError
    }
    public class MarketException : Exception
    {
        private static readonly List<string> publishedErrorIDs = new List<string>();
        private string errorMessage;
        public int  Status { get; }
        public MarketException(int status,string message)
        {
            initiateException(message);
            Status = status;

        }

        public MarketException(MarketError error,string message)
        {
            Status = (int)error;
            initiateException(message);
        }

        private void initiateException(string message)
        {
            string errorID = GenerateErrorID();
            InsertError(errorID);
            errorMessage = message;
            publishedErrorIDs.Add(errorID);
        }

        private void InsertError(string errorID)
        {
            var dbConnection = MarketDB.Instance;
            dbConnection.InsertTable("System_Errors", "ErrorID, ModuleName, Description", 
                new[] { "@idParam", "@moduleParam", "@descriptionParam" }, 
                new object []{errorID ,GetModuleName(),WrapErrorMessageForDb(errorMessage)});
        }

        private static string GenerateErrorID()
        {
            var random = new Random();
            return random.Next(1000, 10000) + "" + ((char)random.Next(97, 123)) + "" + ((char)random.Next(97, 123));
        }

        public string GetErrorMessage()
        {
            return errorMessage;
        }

        protected virtual string GetModuleName()
        {
            return "MarketYard";
        }

        protected virtual string WrapErrorMessageForDb(string message)
        {
            return "General Error: " + message;
        }

        public static bool hasErrorRaised()
        {
            return publishedErrorIDs.Count > 0;
        }

        public static void RemoveErrors()
        {
            var dbConnection = MarketDB.Instance;
            foreach (var errorID in publishedErrorIDs)
            {
                dbConnection.DeleteFromTable("System_Errors","ErrorID = '"+errorID+"'");
            }
            publishedErrorIDs.Clear();
        }
    }
}
