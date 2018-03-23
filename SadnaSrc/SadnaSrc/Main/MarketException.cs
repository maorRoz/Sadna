using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketYard
{
    class MarketException : Exception
    {
        private static object dbConnection = null; 
        private MarketException(string Message)
        {
            dbConnection.       
        }
        public static void insertDbConnector(SQLiteConnection dbConnection)
        {
            MarketException.dbConnection = dbConnection;
        }
    }
}
