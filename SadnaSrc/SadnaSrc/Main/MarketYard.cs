using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.UserSpot;

namespace SadnaSrc.Main
{
    class MarketYard
    {
        public MarketYard()
        {
            initiateDb();
        }

        private void initiateDb()
        {
            var programPath = System.AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", "");
            var dbPath = "URI=file:" + programPath + "MarketYardDB.db";
            var dbConnection = new SQLiteConnection(dbPath);
            dbConnection.Open();
            MarketException.InsertDbConnector(dbConnection);
        }

        public IUserService getUserService()
        {
            return new UserService();
        }
    }
}
