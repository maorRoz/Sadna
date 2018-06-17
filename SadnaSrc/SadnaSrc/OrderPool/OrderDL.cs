using System;
using System.Collections.Generic;
using SadnaSrc.Main;
using SadnaSrc.MarketData;

namespace SadnaSrc.OrderPool
{

    public class OrderDL : IOrderDL
    {
        private static Random rand = new Random();
        private static OrderDL _instance;

        public static OrderDL Instance => _instance ?? (_instance = new OrderDL());

        private IMarketDB dbConnection;

        private OrderDL()
        {
            dbConnection = new ProxyMarketDB();
        }

        public int RandomOrderID()
        {
            var ret = rand.Next(100000, 999999);
            while (FindOrder(ret) != null)
            {
                ret = rand.Next(100000, 999999);
            }

            return ret;
        }


        public Order FindOrder(int orderId)
        {
            Order order = null;
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Orders", "*", "OrderID = " + orderId + ""))
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
            using (var dbReader = dbConnection.SelectFromTable("Orders", "*"))
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
            using (var dbReader = dbConnection.SelectFromTableWithCondition("OrderItem", "*", "OrderID = " + orderId + ""))
            {
                while (dbReader.Read())
                {
                    if (dbReader.GetValue(0) != null)
                    {
                        list.Add(new OrderItem(dbReader.GetString(1), null, dbReader.GetString(2), dbReader.GetDouble(3), dbReader.GetInt32(4)));
                    }
                }
            }
            return list;
        }

        public OrderItem FindOrderItemInOrder(int orderId, string store,string name)
        {
            dbConnection.CheckInput(store); dbConnection.CheckInput(name);
            using (var dbReader = dbConnection.SelectFromTableWithCondition("OrderItem", "*", "OrderID = " + orderId + " AND " +
                                                                          "Store = '" + store + "' AND " +
                                                                          "Name = '" + name + "'"))
            {    while (dbReader.Read())
                {
                    if (dbReader.GetValue(0) != null)
                    {
                        return new OrderItem(dbReader.GetString(1), null, dbReader.GetString(2), dbReader.GetDouble(3), dbReader.GetInt32(4));

                    }
                }
            }
            return null;
        }

        public List<OrderItem> FindOrderItemsFromStore(string store)
        {
            dbConnection.CheckInput(store);
            List<OrderItem> res = new List<OrderItem>();
            using (var dbReader = dbConnection.SelectFromTableWithCondition("OrderItem", "*", "Store = '" + store + "'"))
            {
                while (dbReader.Read())
                {
                    if (dbReader.GetValue(0) != null)
                    {
                        res.Add(new OrderItem(dbReader.GetString(1), null, dbReader.GetString(2), dbReader.GetDouble(3), dbReader.GetInt32(4)));

                    }
                }

            }
            return res;
        }

        public void AddOrder(Order order)
        {
            string[] valuesNames = { "@orderidParam", "@nameParam", "@addressParam", "@priceParam" , "@dateParam" };
            object[] values = order.ToData();
            foreach (object val in values)
                dbConnection.CheckInput(val.ToString());

            dbConnection.InsertTable("Orders", "OrderID,UserName,ShippingAddress,TotalPrice,Date", valuesNames, values);

            foreach (OrderItem item in order.GetItems())
            {
                string[] valuesNames2 = { "@orderidParam", "@storeParam", "@nameParam", "@priceParam", "@quantityParam" };
                object[] values2 = { order.GetOrderID(), item.Store, item.Name, item.Price,item.Quantity };
                foreach (object val in values2)
                    dbConnection.CheckInput(val.ToString());
                dbConnection.InsertTable("OrderItem", "OrderID,Store,Name,Price,Quantity", valuesNames2, values2);


                string[] valuesNames3 = { "@usernameParam", "@productParam", "@storeParam", "@saleParam", "@quantityParam", "@priceParam", "@dateParam" };
                object[] values3 = { order.GetUserName(), item.Name, item.Store, "Immediate", item.Quantity, item.Price, order.GetDate() };
                foreach (object val in values3)
                    dbConnection.CheckInput(val.ToString());
                dbConnection.InsertTable("PurchaseHistory", "UserName,Product,Store,SaleType,Quantity,Price,Date", valuesNames3, values3);
            }
        }

        public void AddOrder(Order order, string saleType)
        {
            dbConnection.CheckInput(saleType);
            string[] valuesNames = { "@orderidParam", "@nameParam", "@addressParam", "@priceParam", "@dateParam" };
            object[] values = order.ToData();
            foreach (object val in values)
                dbConnection.CheckInput(val.ToString());
            dbConnection.InsertTable("Orders", "OrderID,UserName,ShippingAddress,TotalPrice,Date", valuesNames, values);

            foreach (OrderItem item in order.GetItems())
            {
                string[] valuesNames2 = { "@orderidParam", "@storeParam", "@nameParam", "@priceParam", "@quantityParam" };
                object[] values2 = { order.GetOrderID(), item.Store, item.Name, item.Price, item.Quantity };
                foreach (object val in values2)
                    dbConnection.CheckInput(val.ToString());
                dbConnection.InsertTable("OrderItem", "OrderID,Store,Name,Price,Quantity", valuesNames2, values2);

                
                string[] valuesNames3 = { "@usernameParam", "@productParam", "@storeParam", "@saleParam", "@quantityParam", "@priceParam", "@dateParam" };
                object[] values3 = { order.GetUserName(), item.Name, item.Store, saleType,item.Quantity,item.Price, order.GetDate() };
                foreach (object val in values3)
                    dbConnection.CheckInput(val.ToString());
                dbConnection.InsertTable("PurchaseHistory", "UserName,Product,Store,SaleType,Quantity,Price,Date", valuesNames3, values3);
            }

        }

        public void RemoveOrder(int orderId)
        {
            List<OrderItem> items = GetAllItems(orderId);
            foreach (OrderItem item in items)
            {
                dbConnection.CheckInput(item.Name); dbConnection.CheckInput(item.Store);
                dbConnection.DeleteFromTable("PurchaseHistory", "Product = '" + item.Name +"' AND Store = '"+ item.Store +"'");
            }
            dbConnection.DeleteFromTable("OrderItem", "OrderID = " + orderId);
            dbConnection.DeleteFromTable("Orders", "OrderID = " + orderId);

        }

        public void AddItemToOrder(int orderId, OrderItem item)
        {
            dbConnection.CheckInput(item.Name); dbConnection.CheckInput(item.Store);
            string[] valuesNames2 = { "@OrderIdParam", "@StoreParam", "@NameParam", "@PriceParam", "@quantityParam" };
            object[] values2 = { orderId, item.Store, item.Name, item.Price, item.Quantity };
            dbConnection.InsertTable("OrderItem", "OrderID,Store,Name,Price,Quantity", valuesNames2, values2);
        }

        public void RemoveItemFromOrder(int orderId, string name, string store)
        {
            dbConnection.CheckInput(name); dbConnection.CheckInput(store);
            dbConnection.DeleteFromTable("OrderItem", "OrderID = " + orderId + " AND Name = '" + name + "' AND Store = '" + store + "'");
        }


        public void UpdateOrderPrice(int orderId, double price)
        {
            string[] columnNames = { "TotalPrice"};
            string[] valuesNames = { "@totalprice"};
            object[] values = { price };
            dbConnection.UpdateTable("Orders", "OrderID = " + orderId, columnNames, valuesNames, values);
        }

        public string[] GetAllExpiredLotteries()
        {
            List<string> expiredLotteries = new List<string>();
            using (var dbReader = dbConnection.SelectFromTableWithCondition("LotteryTable", "SystemID,EndDate", "isActive = 'true'"))
            {
                while (dbReader.Read())
                {
                    string lotteryID = dbReader.GetString(0);
                    DateTime endDate = dbReader.GetDateTime(1);
                    if (endDate < MarketYard.MarketDate)
                    {
                        expiredLotteries.Add(lotteryID);
                    }
                }
            }

            return expiredLotteries.ToArray();
        }

        public void CancelLottery(string lottery)
        {
            dbConnection.CheckInput(lottery);
            dbConnection.UpdateTable("LotteryTable", "SystemID = '" + lottery + "'",
                new[] {"IsActive"},new []{"@status"},new object []{"false"});

        }



        public string[] GetAllTickets(string lottery)
        {
            dbConnection.CheckInput(lottery);
            List<string> tickets = new List<string>();
            using (var dbReader = dbConnection.SelectFromTableWithCondition("LotteryTicket", "myID","LotteryID = '"+lottery +"'"))
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
            dbConnection.CheckInput(ticket);
            using (var dbReader = dbConnection.SelectFromTableWithCondition("LotteryTicket", "UserID", "myID ='" + ticket + "'" ))
            {
                if (dbReader.Read())
                {
                    return dbReader.GetInt32(0);
                }
                return -1;
            }
        }
        
        public string GetCreditCardToRefund(int userID)
        {
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Users", "CreditCard", "SystemID ='" + userID + "'"))
            {
                if (dbReader.Read())
                {
                    return dbReader.GetString(0);
                }
                return null;
            }
        }
        
        public string GetNameToRefund(int userID)
        {
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Users", "Name", "SystemID ='" + userID + "'"))
            {
                if (dbReader.Read())
                {
                    return dbReader.GetString(0);
                }
                return null;
            }
        }
        
        public string GetAddressToSendPackage(int userID)
        {
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Users", "Name", "SystemID ='" + userID + "'"))
            {
                if (dbReader.Read())
                {
                    return dbReader.GetString(0);
                }

                return null;
                
            }
        }

        public double GetSumToRefund(string ticket)
        {
            dbConnection.CheckInput(ticket);
            using (var dbReader = dbConnection.SelectFromTableWithCondition("LotteryTicket", "Cost", "myID ='" + ticket + "'"))
            {
                if (dbReader.Read())
                {
                    return dbReader.GetDouble(0);
                }

                return -1;
            }
        }

        public void RemoveTicket(string ticket)
        {
            dbConnection.CheckInput(ticket);
            dbConnection.DeleteFromTable("LotteryTicket", "myID ='" + ticket + "'");
        }


    }

}
