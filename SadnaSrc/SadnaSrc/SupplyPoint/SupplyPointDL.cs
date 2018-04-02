using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;

namespace SadnaSrc.SupplyPoint
{
    public class SupplyPointDL : SystemDL
    {
        public SupplyPointDL(SQLiteConnection dbConnection) : base(dbConnection)
        {
        }

        public void AddDelivery(Order order)
        {

        }

        public bool FindDelivery(int orderId)
        {
            return true;
        }

        public void MarkOrderDelivered(int orderId)
        {

        }
    }
}
