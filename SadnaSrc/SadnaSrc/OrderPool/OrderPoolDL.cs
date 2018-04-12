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
            using (var dbReader = SelectFromTableWithCondition("Orders", "*", "OrderID = " + orderId + ""))
            {
                while (dbReader.Read())
                {
                    if (dbReader.GetValue(0) != null)
                    {
                        order = new Order(dbReader.GetInt32(0), dbReader.GetString(1), dbReader.GetString(2), dbReader.GetDouble(3)
                            , dbReader.GetString(4), GetAllItems(orderId));
                    }
                }
            }
            return order;
        }

        public List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();
            using (var dbReader = SelectFromTable("Orders", "*"))
            {
                while (dbReader.Read())
                {
                    if (dbReader.GetValue(0) != null)
                    {
                        orders.Add(new Order(dbReader.GetInt32(0), dbReader.GetString(1), dbReader.GetString(2), dbReader.GetDouble(3)
                            , dbReader.GetString(4), GetAllItems(dbReader.GetInt32(0))));
                    }
                }

            }
            return orders;
        }

        public List<OrderItem> GetAllItems(int orderId)
        {
            List<OrderItem> list = new List<OrderItem>();
            using (var dbReader = SelectFromTableWithCondition("OrderItem", "*", "OrderID = " + orderId + ""))
            {
                while (dbReader.Read())
                {
                    if (dbReader.GetValue(0) != null)
                    {
                        list.Add(new OrderItem(dbReader.GetString(1), dbReader.GetString(2), dbReader.GetDouble(3), dbReader.GetInt32(4)));
                    }
                }
            }
            return list;
        }

        public OrderItem FindOrderItemInOrder(int orderId, string store,string name)
        {
            using (var dbReader = SelectFromTableWithCondition("OrderItem", "*", "OrderID = " + orderId + " AND " +
                                                                          "Store = '" + store + "' AND " +
                                                                          "Name = '" + name + "'"))
            {    while (dbReader.Read())
                {
                    if (dbReader.GetValue(0) != null)
                    {
                        return new OrderItem(dbReader.GetString(1), dbReader.GetString(2), dbReader.GetDouble(3), dbReader.GetInt32(4));

                    }
                }
            }
            return null;
        }

        public List<OrderItem> FindOrderItemsFromStore(string store)
        {
            List<OrderItem> res = new List<OrderItem>();
            using (var dbReader = SelectFromTableWithCondition("OrderItem", "*", "Store = '" + store + "'"))
            {
                while (dbReader.Read())
                {
                    if (dbReader.GetValue(0) != null)
                    {
                        res.Add(new OrderItem(dbReader.GetString(1), dbReader.GetString(2), dbReader.GetDouble(3), dbReader.GetInt32(4)));

                    }
                }

            }
            return res;
        }

        public void AddOrder(Order order)
        {
            string[] valuesNames = { "@orderidParam", "@nameParam", "@addressParam", "@priceParam" , "@dateParam" };
            object[] values = order.ToData();
            InsertTable("Orders", "OrderID,UserName,ShippingAddress,TotalPrice,Date", valuesNames, values);

            foreach (OrderItem item in order.GetItems())
            {
                string[] valuesNames2 = { "@orderidParam", "@storeParam", "@nameParam", "@priceParam", "@quantityParam" };
                object[] values2 = { order.GetOrderID(), item.Store, item.Name, item.Price,item.Quantity };
                InsertTable("OrderItem", "OrderID,Store,Name,Price,Quantity", valuesNames2, values2);
                
                //TODO: add this after branch 3982 rebase
                /*
                string[] valuesNames3 = { "@usernameParam", "@productParam", "@storeParam", "@saleParam", "@dateParam" };
                object[] values3 = { order.GetUserName(), item.Store, item.Name, "Immediate", order.GetDate().ToString("dd/MM/yyyy") };
                InsertTable("PurchaseHistory", "OrderID,Store,Name,Price,Quantity", valuesNames3, values3);*/
            }
        }

        public void AddOrder(Order order, string SaleType)
        {
            string[] valuesNames = { "@orderidParam", "@nameParam", "@addressParam", "@priceParam", "@dateParam" };
            object[] values = order.ToData();
            InsertTable("Orders", "OrderID,UserName,ShippingAddress,TotalPrice,Date", valuesNames, values);

            foreach (OrderItem item in order.GetItems())
            {
                string[] valuesNames2 = { "@orderidParam", "@storeParam", "@nameParam", "@priceParam", "@quantityParam" };
                object[] values2 = { order.GetOrderID(), item.Store, item.Name, item.Price, item.Quantity };
                InsertTable("OrderItem", "OrderID,Store,Name,Price,Quantity", valuesNames2, values2);

                //TODO: add this after branch 3982 rebase
                /*
                string[] valuesNames3 = { "@usernameParam", "@productParam", "@storeParam", "@saleParam", "@dateParam" };
                object[] values3 = { order.GetUserName(), item.Store, item.Name, SaleType, order.GetDate().ToString("dd/MM/yyyy") };
                InsertTable("OrderItem", "OrderID,Store,Name,Price,Quantity", valuesNames3, values3);*/
            }

        }

        public void RemoveOrder(int orderId)
        {
            
            DeleteFromTable("OrderItem", "OrderID = " + orderId);
            DeleteFromTable("Orders", "OrderID = " + orderId);

        }

        public void AddItemToOrder(int orderId, OrderItem item)
        {
            string[] valuesNames2 = { "@OrderIdParam", "@StoreParam", "@NameParam", "@PriceParam", "@quantityParam" };
            object[] values2 = { orderId, item.Store, item.Name, item.Price, item.Quantity };
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

        public string[] GetAllExpiredLotteries()
        {
            List<string> expiredLotteries = new List<string>();
            using (var dbReader = SelectFromTableWithCondition("LotteryTable", "SystemID,EndDate", "isActive = 'true'"))
            {
                while (dbReader.Read())
                {
                    string lotteryID = dbReader.GetString(0);
                    DateTime endDate = dbReader.GetDateTime(1);
                    if (endDate > MarketYard.MarketDate)
                    {
                        expiredLotteries.Add(lotteryID);
                    }
                }
            }

            return expiredLotteries.ToArray();
        }

        public string[] GetAllTickets(string lottery)
        {
            List<string> tickets = new List<string>();
            using (var dbReader = SelectFromTableWithCondition("LotteryTicket", "myID","LotteryID = '"+lottery +"'"))
            {
                while (dbReader.Read())
                {
                    tickets.Add(dbReader.GetString(0));
                }
            }

            return tickets.ToArray();
        }

        public int GetTicketParticipantID(string ticket)
        {
            using (var dbReader = SelectFromTableWithCondition("LotteryTicket", "UserID", "myID ='" + ticket + "'"))
            {
                if (dbReader.Read())
                {
                    return dbReader.GetInt32(0);
                }
                throw new OrderException(OrderItemStatus.InvalidDetails, "Cannot find ticket or user");
            }
        }

        public string GetCreditCardToRefund(int userID)
        {
            using (var dbReader = SelectFromTableWithCondition("User", "CreditCard", "SystemID ='" + userID + "'"))
            {
                if (dbReader.Read())
                {
                    return dbReader.GetString(0);
                }
                throw new OrderException(OrderItemStatus.InvalidDetails, "Cannot find credit card or user");
            }
        }

        public string GetNameToRefund(int userID)
        {
            using (var dbReader = SelectFromTableWithCondition("User", "Name", "SystemID ='" + userID + "'"))
            {
                if (dbReader.Read())
                {
                    return dbReader.GetString(0);
                }
                throw new OrderException(OrderItemStatus.InvalidDetails, "Cannot find name or user");
            }
        }

        public double GetSumToRefund(string ticket)
        {
            using (var dbReader = SelectFromTableWithCondition("LotteryTicket", "Cost", "myID ='" + ticket + "'"))
            {
                if (dbReader.Read())
                {
                    return dbReader.GetDouble(0);
                }
                throw new OrderException(OrderItemStatus.InvalidDetails,"Cannot find cost or ticket");
            }
        }

        public void RemoveTicket(string ticket)
        {
            DeleteFromTable("LotteryTicket","myID = '"+ticket+"'");
        }


    }

}
