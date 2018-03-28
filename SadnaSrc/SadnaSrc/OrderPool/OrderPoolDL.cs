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

        public Order FindOrder(int id)
        {
            Order order = null;
            var dbReader = SelectFromTableWithCondition("Order", "*", "ID = " + id + "");
            while (dbReader.Read())
            {
                if (dbReader.GetValue(0) != null)
                {
                    order = new Order(dbReader.GetInt32(0), dbReader.GetString(1), dbReader.GetString(2));
                }
            }
            return order;
        }

        public List<OrderItem> FindOrderItemsOfOrder(int id)
        {
            List<OrderItem> list = new List<OrderItem>();
            var dbReader = SelectFromTableWithCondition("OrderItemsToOrders JOIN OrderItem", "*", "OrderID = " + id + "");
            while (dbReader.Read())
            {
                if (dbReader.GetValue(0) != null)
                {
                    //list.Add(new OrderItem( args ));
                    // TODO see how the joined table is buikd and add the order items accordingly
                }
            }
            return list;
        }

        public void AddOrder(Order order)
        {

        }
    }
}
