using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.MarketData;
using SadnaSrc.MarketRecovery;

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

        private static IMarketBackUpDB _dbConnection = MarketBackUpDB.Instance;


        public static void SetDB(IMarketBackUpDB dbConnection)
        {
            _dbConnection = dbConnection;
        }
        public MarketException(int status,string message)
        {
            InitiateException(GetModuleName(),WrapErrorMessageForDb(message));
            Status = status;

        }

        public MarketException(MarketError error,string message)
        {
            InitiateException(GetModuleName(), WrapErrorMessageForDb(message));
            Status = (int)error;
        }

        private void InitiateException(string moduleName,string message)
        {
            var allErrorIds = GetAllErrorsIds();
            var errorId = GenerateErrorID();
            while (allErrorIds.Contains(errorId))
            {
                errorId = GenerateErrorID();
            }

            InsertError(errorId, moduleName, message);
            errorMessage = message;
            publishedErrorIDs.Add(errorId);
        }

        private void InsertError(string errorID,string moduleName,string message)
        {
                _dbConnection.InsertTable("System_Errors", "ErrorID, ErrorDate, ModuleName, Description",
                    new[] {"@idParam", "@dateParam","@moduleParam", "@descriptionParam"},
                    new object[] {errorID, DateTime.Now, moduleName, message});
        }

        private static string[] GetAllErrorsIds()
        {
            var ids = new List<string>();
            using (var dbReader = _dbConnection.SelectFromTable("System_Errors", "ErrorID"))
            {
                while (dbReader != null && dbReader.Read() )
                {
                    if (dbReader.GetValue(0) != null)
                    {
                        ids.Add(dbReader.GetString(0));
                    }
                }
            }

            return ids.ToArray();
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

        public static bool HasErrorRaised()
        {
            return publishedErrorIDs.Count > 0;
        }

        public static void RemoveErrors()
        {
            publishedErrorIDs.Clear();
        }
    }
}
