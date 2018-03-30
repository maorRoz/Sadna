using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.OrderPool
{

    // TODO add Logging mechanism and Order History DB Tables(or table) to the OrderPool
    class OrderPoolDL : SystemDL
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
                    // TODO fix the order construction
                    order = new Order(dbReader.GetInt32(0), dbReader.GetString(1), dbReader.GetString(2));
                }
            }
            return order;
        }

        public List<OrderItem> GetAllItems(int orderId)
        {
            List<OrderItem> list = new List<OrderItem>();
            var dbReader = SelectFromTableWithCondition("OrderItem", "*", "OrderID = " + orderId + "");
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

        public OrderItem FindOrderItemInOrder(int orderId, string store,string name)
        {
            var dbReader = SelectFromTableWithCondition("OrderItem", "*", "OrderID = " + orderId + "AND"+
                                                                          "Store = '" + store + "' AND "+
                                                                          "Name = '"+ name + "'");
            while (dbReader.Read())
            {
                if (dbReader.GetValue(0) != null)
                {
                    return new OrderItem(dbReader.GetString(0),dbReader.GetString(1),dbReader.GetInt32(2), dbReader.GetInt32(3));
                    
                }
            }
            return null;
        }

        public void AddOrder(Order order)
        {
            string[] valuesNames = { "@idParam", "@nameParam", "@addressParam", "@priceParam" , "@dateParam" };
            object[] values = order.ToData();
            InsertTable("Order", "ID,Username,ShippingAddress,TotalPrice,Date", valuesNames, values);

            foreach (OrderItem item in order.GetItems())
            {
                string[] valuesNames2 = { "@idParam", "@storeParam", "@nameParam", "@priceParam", "@quantityParam" };
                object[] values2 = { order.GetOrderID(), item.GetStore(), item.GetName(), item.GetPrice(),item.GetQuantity() };
                InsertTable("OrderItem", "OrderID,Store,Name,Price,Quantity", valuesNames2, values2);
            }

        }

        public void RemoveOrder(int orderId)
        {
            if (FindOrder(orderId) == null)
            {
                throw new OrderException(0,"no user with id: "+ orderId + " in the Order database.");
            }
            DeleteFromTable("OrderItem", "OrderID = " + orderId);
            DeleteFromTable("Order", "ID = " + orderId);

        }

        public void AddItemToOrder(int orderId, OrderItem item)
        {
            string[] valuesNames2 = { "@idParam", "@storeParam", "@nameParam", "@priceParam", "@quantityParam" };
            object[] values2 = { orderId, item.GetStore(), item.GetName(), item.GetPrice(), item.GetQuantity() };
            InsertTable("OrderItem", "OrderID,Store,Name,Price,Quantity", valuesNames2, values2);
        }

        public void RemoveItemFromOrder(int orderId, string name, string store)
        {
            DeleteFromTable("OrderItem", "OrderID = " + orderId + "Name = '" + name + "' AND Store = '" + store + "'");
        }


        public void UpdateOrderPrice(int orderId, double price)
        {
            string[] columnNames = { "TotalPrice"};
            string[] valuesNames = { "@totalprice"};
            object[] values = { price };
            UpdateTable("Order", "OrderID = " + orderId, columnNames, valuesNames, values);
        }
    }
}
