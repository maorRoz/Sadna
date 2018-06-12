using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketRecovery
{
    public interface IMarketBackUpDB
    {
        void InsertTable(string table, string tableColumns, string[] valuesNames, object[] values);

        SQLiteDataReader SelectFromTable(string table, string toSelect);
    }
}
