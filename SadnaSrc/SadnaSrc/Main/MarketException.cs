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
        private static readonly Random random = new Random();
        private string errorMessage;
        public int  Status { get; }

        private static IMarketDB _dbConnection = MarketDB.Instance;


        public static void SetDB(IMarketDB dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public MarketException(int status,string message)
        {
            InitiateException(message);
            Status = status;

        }

        public MarketException(MarketError error,string message)
        {
            Status = (int)error;
            InitiateException(message);
        }

        private void InitiateException(string message)
        {
            string errorID = GenerateErrorID();
            InsertError(errorID);
            errorMessage = message;
            publishedErrorIDs.Add(errorID);
        }

        private void InsertError(string errorID)
        {
            _dbConnection.InsertTable("System_Errors", "ErrorID, ModuleName, Description", 
                new[] { "@idParam", "@moduleParam", "@descriptionParam" }, 
                new object []{errorID ,GetModuleName(),WrapErrorMessageForDb(errorMessage)});
        }

        private static string GenerateErrorID()
        {
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
            foreach (var errorID in publishedErrorIDs)
            {
                _dbConnection.DeleteFromTable("System_Errors","ErrorID = '"+errorID+"'");
            }
            publishedErrorIDs.Clear();
        }
    }
}
