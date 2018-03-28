using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.OrderPool
{
    class OrderPoolDL : systemDL
    {
        public OrderPoolDL(SQLiteConnection dbConnection) : base(dbConnection)
        {
        }
    }
}
