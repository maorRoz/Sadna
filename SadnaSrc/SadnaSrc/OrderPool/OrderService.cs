using SadnaSrc.UserSpot;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.OrderPool
{
    public class OrderService : IOrderService
    {
        private string _userName;
        private readonly OrderPoolDL _orderDL;
        private List<Order> _orders;

        public string getUsername() {  return _userName;}
        public List<Order> getOrders() { return  _orders; }

        public void setUsername(string name) { _userName = name;}


        public OrderService(string userName, SQLiteConnection dbConnection)
        {
            _orders= new List<Order>();
            _userName = userName;
            _orderDL = new OrderPoolDL(dbConnection);
        }

        public OrderService(SQLiteConnection dbConnection)
        {
            _orders = new List<Order>();
            _userName = "default";
            _orderDL = new OrderPoolDL(dbConnection);
        }


        public Order CreateOrder(OrderItem[] items)
        {
            Order order = new Order(RandomOrderID(), _userName);
            foreach (OrderItem item in items)
            {
                order.AddOrderItem(item);
            }

            _orders.Add(order);
            _orderDL.AddOrder(order);

            MarketLog.Log("OrderPool", "User " + _userName + " added new order from the cart.");
            return order;
        }

        public Order CreateOrder()
        {
            Order order = new Order(RandomOrderID(), _userName);

            _orders.Add(order);
            _orderDL.AddOrder(order);

            MarketLog.Log("OrderPool", "User " + _userName + " added new order.");

            return order;
        }

        public Order getOrder(int orderID)
        {
            foreach (Order order in _orders)
            {
                if (order.GetOrderID() == orderID) return order;
            }

            return null;
        }


        public void RemoveOrder(int orderId)
        {
            foreach (Order order in _orders)
            {
                if (order.GetOrderID() == orderId)
                {
                    _orders.Remove(order);
                    _orderDL.RemoveOrder(orderId);
                    MarketLog.Log("OrderPool", "User " + _userName + " removed order ID: "+orderId+" from his OrderPool");
                    return;
                }
            } 
        }

        public void RemoveItemFromOrder(int orderID, string store, string name)
        {
            foreach (Order order in _orders)
            {
                if(order.GetOrderID() == orderID)
                {
                    var item = order.getOrderItem(name, store);
                    if (item != null)
                    {
                        double newPrice = order.GetPrice() - item.GetPrice() * item.GetQuantity();
                        order.RemoveOrderItem(item);
                        _orderDL.RemoveItemFromOrder(orderID, name, store);
                        _orderDL.UpdateOrderPrice(orderID, newPrice);
                        MarketLog.Log("OrderPool", "User " + _userName + " removed order Item from order ID: " + orderID);
                        return;
                    }

                }
            }
        }

        public void AddItemToOrder(int orderID, OrderItem item)
        {
            var order = getOrder(orderID);
            double newPrice = order.GetPrice() + item.GetPrice() * item.GetQuantity();

            order.AddOrderItem(item);
            order.setPrice(newPrice);
            _orderDL.AddItemToOrder(orderID, item);
            _orderDL.UpdateOrderPrice(orderID, newPrice);
            MarketLog.Log("OrderPool", "User " + _userName + " added order Item to order ID: " + orderID);

        }

        private int RandomOrderID()
        {
            var ret = new Random().Next(100000, 999999);
            while (_orderDL.FindOrder(ret) != null)
            {
                ret = new Random().Next(100000, 999999);
            }

            return ret;
        }

        public OrderItem FindOrderItemInOrder(int orderId, string store, string user)
        {
            foreach (Order order in _orders)
            {
                return order.getOrderItem(user, store);
            }

            return null;
        }


    }
}
