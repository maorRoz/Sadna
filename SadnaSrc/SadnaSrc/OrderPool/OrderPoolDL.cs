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
            string[] valuesNames = { "@idParam", "@nameParam", "@addressParam", "@priceParam" };
            object[] values = order.ToData();
            InsertTable("Order", "ID,Username,ShippingAddress,TotalPrice", valuesNames, values);

            foreach (OrderItem item in order.GetItems())
            {
                string[] valuesNames2 = { "@idParam", "@storeParam", "@nameParam" };
                object[] values2 = { order.GetOrderID(), item.GetStore(), item.GetName()};
                InsertTable("OrderItemsToOrders", "OrderID,Store,Name", valuesNames2, values2);

                string[] valuesNames3 = {"@storeParam", "@nameParam", "@priceParam", "@quantityParam" };
                object[] values3 = { item.GetStore(), item.GetName(), item.GetPrice(),item.GetQuantity() };
                InsertTable("OrderItem", "Store,Name,Price,Quantity", valuesNames3, values3);
            }

        }

        public void RemoveOrder(int orderId)
        {
            if (FindOrder(orderId) == null)
            {
                throw new OrderException("no user with id: "+ orderId + " in the Order database.");
            }
            List<OrderItem> list = FindOrderItemsOfOrder(orderId);
            foreach (OrderItem item in list)
            {
                DeleteFromTable("OrderItem", "Store = '" + item.GetStore() + "' AND Name = '"+ item.GetName() +"'");
            }
            DeleteFromTable("OrderItemsToOrders", "OrderID = " + orderId);
            DeleteFromTable("Order", "ID = " + orderId);

        }

        public void AddItemToOrder(int orderId, OrderItem item)
        {
            string[] valuesNames2 = { "@idParam", "@storeParam", "@nameParam" };
            object[] values2 = { orderId, item.GetStore(), item.GetName() };
            InsertTable("OrderItemsToOrders", "OrderID,Store,Name", valuesNames2, values2);

            string[] valuesNames3 = { "@storeParam", "@nameParam", "@priceParam", "@quantityParam" };
            object[] values3 = { item.GetStore(), item.GetName(), item.GetPrice(), item.GetQuantity() };
            InsertTable("OrderItem", "Store,Name,Price,Quantity", valuesNames3, values3);
        }

        public void RemoveItemFromOrder(int orderId, string name, string store)
        {
            DeleteFromTable("OrderItemsToOrders", "OrderID = " + orderId + " AND Name = '"+ name +"' AND Store = '"+ store +"'");
            DeleteFromTable("OrderItem","Name = '" + name + "' AND Store = '" + store + "'");
        }
    }
}
