using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    class MarketException : Exception
    {
        private static SQLiteConnection _dbConnection = null;
        private static int _id;
        protected string moduleName = "MarketYard";
        public MarketException(string message)
        {
            var insertRequest = "INSERT INTO System_Errors (ErrorID,ModuleName,Description) VALUES (@idParam,@moduleParam,@descriptionParam)";
            var commandDb = new SQLiteCommand(insertRequest, _dbConnection);
            commandDb.Parameters.AddWithValue("@idParam",_id);
            commandDb.Parameters.AddWithValue("@moduleParam", GetModuleName());
            commandDb.Parameters.AddWithValue("@descriptionParam", GetErrorMessage(message));
            try
            {
                commandDb.ExecuteNonQuery();
                _id++;
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem occured in the attempt to save system error in DB, returned error message :"+e.Message);
            }

        }

        protected virtual string GetModuleName()
        {
            return "MarketYard";
        }

        protected virtual string GetErrorMessage(string message)
        {
            return "General Error: " + message;
        }
        public static void InsertDbConnector(SQLiteConnection dbConnection)
        {
            if (_dbConnection == null)
            {
                _dbConnection = dbConnection;
                var selectRequest = "SELECT COALESCE(MAX(ErrorID),0) FROM System_Errors";
                var commandDb = new SQLiteCommand(selectRequest, _dbConnection);
                try
                {
                    using (var dbReader = commandDb.ExecuteReader())
                    {
                        while (dbReader.Read())
                        {
                            if (dbReader.GetValue(0) != null)
                            {
                                _id = dbReader.GetInt32(0) + 1;
                            }
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
