using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.OrderPool
{

    class OrderPoolDL : SystemDL
    {

        public Order FindOrder(int orderId)
        {
            Order order = null;
            var dbReader = SelectFromTableWithCondition("Orders", "*", "OrderID = " + orderId + "");
            while (dbReader.Read())
            {
                if (dbReader.GetValue(0) != null)
                {
                    order = new Order(dbReader.GetInt32(0), dbReader.GetString(1), dbReader.GetString(2), dbReader.GetDouble(3)
                    , dbReader.GetString(4),GetAllItems(orderId));
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
                    list.Add(new OrderItem(dbReader.GetString(1),dbReader.GetString(2),dbReader.GetDouble(3), dbReader.GetInt32(4)));
                }
            }
            return list;
        }

        public OrderItem FindOrderItemInOrder(int orderId, string store,string name)
        {
            var dbReader = SelectFromTableWithCondition("OrderItem", "*", "OrderID = " + orderId + " AND "+
                                                                          "Store = '" + store + "' AND "+
                                                                          "Name = '"+ name + "'");
            while (dbReader.Read())
            {
                if (dbReader.GetValue(0) != null)
                {
                    return new OrderItem(dbReader.GetString(1),dbReader.GetString(2),dbReader.GetDouble(3), dbReader.GetInt32(4));
                    
                }
            }
            return null;
        }

        public void AddOrder(Order order)
        {
            string[] valuesNames = { "@orderidParam", "@nameParam", "@addressParam", "@priceParam" , "@dateParam" };
            object[] values = order.ToData();
            InsertTable("Orders", "OrderID,UserName,ShippingAddress,TotalPrice,Date", valuesNames, values);

            foreach (OrderItem item in order.GetItems())
            {
                string[] valuesNames2 = { "@orderidParam", "@storeParam", "@nameParam", "@priceParam", "@quantityParam" };
                object[] values2 = { order.GetOrderID(), item.GetStore(), item.GetName(), item.GetPrice(),item.GetQuantity() };
                InsertTable("OrderItem", "OrderID,Store,Name,Price,Quantity", valuesNames2, values2);
            }

        }

        public void RemoveOrder(int orderId)
        {
            if (FindOrder(orderId) == null)
            {
                throw new OrderException(OrderStatus.NoOrderWithID,"no user with id: "+ orderId + " in the Order database.");
            }
            DeleteFromTable("OrderItem", "OrderID = " + orderId);
            DeleteFromTable("Orders", "OrderID = " + orderId);

        }

        public void AddItemToOrder(int orderId, OrderItem item)
        {
            string[] valuesNames2 = { "@OrderIdParam", "@StoreParam", "@NameParam", "@PriceParam", "@quantityParam" };
            object[] values2 = { orderId, item.GetStore(), item.GetName(), item.GetPrice(), item.GetQuantity() };
            InsertTable("OrderItem", "OrderID,Store,Name,Price,Quantity", valuesNames2, values2);
        }

        public void RemoveItemFromOrder(int orderId, string name, string store)
        {
            DeleteFromTable("OrderItem", "OrderID = " + orderId + " AND Name = '" + name + "' AND Store = '" + store + "'");
        }


        public void UpdateOrderPrice(int orderId, double price)
        {
            string[] columnNames = { "TotalPrice"};
            string[] valuesNames = { "@totalprice"};
            object[] values = { price };
            UpdateTable("Orders", "OrderID = " + orderId, columnNames, valuesNames, values);
        }
    }
}
