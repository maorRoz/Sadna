using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public interface IMarketDB
    {
         void InsertTable(string table, string tableColumns, string[] valuesNames, object[] values);

         SQLiteDataReader SelectFromTable(string table, string toSelect);


         SQLiteDataReader SelectFromTableWithCondition(string table, string toSelect, string condition);


         void UpdateTable(string table, string updateCondition, string[] columnNames,
            string[] valuesNames, object[] values);


         void DeleteFromTable(string table, string deleteCondition);

         SQLiteDataReader freeStyleSelect(string cmd);

         void Exit();
    }
}
