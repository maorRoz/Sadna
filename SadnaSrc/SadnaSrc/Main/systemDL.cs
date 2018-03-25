using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public class systemDL
    {
        private SQLiteConnection _dbConnection;

        public systemDL(SQLiteConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        protected void InsertTable(string table,string tableColumns,string[] valuesNames,object[] values)
        {
            string valuesNamesJoin = String.Join(",", valuesNames);
            var insertRequest = "INSERT INTO "+table+" ("+ tableColumns + ") VALUES ("+ valuesNamesJoin + ")";
            var commandDb = new SQLiteCommand(insertRequest, _dbConnection);
            for (int i = 0; i < values.Length;i++)
            {
                commandDb.Parameters.AddWithValue(valuesNames[i], values[i]);
            }

            try
            {
                commandDb.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem occured in the attempt to save system data in DB, returned error message :" + e.Message);
            }
        }

        protected SQLiteDataReader SelectFromTable(string table, string toSelect)
        {
            var selectRequest = "SELECT " + toSelect + " FROM " + table;
            return new SQLiteCommand(selectRequest, _dbConnection).ExecuteReader();
        }

        protected void DeleteFromTable(string table,string deleteCondition)
        {
            var deleteCommand = "DELETE FROM " + table + " WHERE " + deleteCondition;
            var commandDb = new SQLiteCommand(deleteCommand, _dbConnection);
            try
            {
                commandDb.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem occured in the attempt to save system data in DB, returned error message :" + e.Message);
            }

        }
        protected void CleanTable(string table)
        {

        }

    }
}
