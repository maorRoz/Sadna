using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public class MarketException : Exception
    {
        private static SQLiteConnection _dbConnection;
        private static List<string> publishedErrorIDs;
        public MarketException(string message)
        {
            var insertRequest = "INSERT INTO System_Errors (ErrorID,ModuleName,Description) VALUES (@idParam,@moduleParam,@descriptionParam)";
            var commandDb = new SQLiteCommand(insertRequest, _dbConnection);
            string errorID = GenerateErrorID();
            commandDb.Parameters.AddWithValue("@idParam", errorID);
            commandDb.Parameters.AddWithValue("@moduleParam", GetModuleName());
            commandDb.Parameters.AddWithValue("@descriptionParam", GetErrorMessage(message));
            try
            {
                commandDb.ExecuteNonQuery();
                publishedErrorIDs.Add(errorID);
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem occured in the attempt to save system error in DB, returned error message :"+e.Message);
            }

        }

        public static void InsertDbConnector(SQLiteConnection dbConnection)
        {
            _dbConnection = dbConnection;
            publishedErrorIDs = new List<string>();
        }

        private static string GenerateErrorID()
        {
            var random = new Random();
            return random.Next(1000, 10000) + "" + ((char)random.Next(97, 123)) + "" + ((char)random.Next(97, 123));
        }

        protected virtual string GetModuleName()
        {
            return "MarketYard";
        }

        protected virtual string GetErrorMessage(string message)
        {
            return "General Error: " + message;
        }

        public static void RemoveErrors()
        {
            foreach (string errorID in publishedErrorIDs)
            {
                var deleteRequest = "DELETE FROM System_Errors WHERE ErrorID LIKE '%" + errorID + "%'";
                var commandDb = new SQLiteCommand(deleteRequest, _dbConnection);
                try
                {
                    commandDb.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(
                        "Problem occured in the attempt to communicate with the DB, returned error message :" +
                        e.Message);
                    break;
                }
            }
            publishedErrorIDs.Clear();
        }
    }
}
