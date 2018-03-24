using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    class systemDL
    {
        private SQLiteConnection _dbConnection;

        public systemDL(SQLiteConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        protected void InsertTable(string table)
        {

        }

        protected void SelectFromTable(string table, string fields)
        {

        }
        protected void RemoveFromTable(string table)
        {

        }
        protected void CleanTable(string table)
        {

        }

    }
}
