using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public interface IMarketDB
    {
         void InsertTable(string table, string tableColumns, string[] valuesNames, object[] values);

        SqlDataReader SelectFromTable(string table, string toSelect);


        SqlDataReader SelectFromTableWithCondition(string table, string toSelect, string condition);


         void UpdateTable(string table, string updateCondition, string[] columnNames,string[] valuesNames, object[] values);
 

         void DeleteFromTable(string table, string deleteCondition);

        SqlDataReader freeStyleSelect(string cmd);

    }
}
