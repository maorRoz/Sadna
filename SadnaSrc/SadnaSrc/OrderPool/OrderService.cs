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
        // TODO add support to external systems once there is more clarification about the systems. !!! IMPORTANT !!!
        private string _userName;
        private readonly OrderPoolDL _orderDL;
        private List<Order> _orders;
        private bool _toSave;

        public string getUsername() {  return _userName;}
        public List<Order> getOrders() { return  _orders; }

        public void setUsername(string name) { _userName = name;}
        public void setSave(bool save) { _toSave = save; }


        public OrderService(string userName, bool toSave, SQLiteConnection dbConnection)
        {
            _orders= new List<Order>();
            _userName = userName;
            _toSave = toSave;
            _orderDL = new OrderPoolDL(dbConnection);
        }

        public OrderService(SQLiteConnection dbConnection)
        {
            _orders = new List<Order>();
            _userName = "default";
            _toSave = true;
            _orderDL = new OrderPoolDL(dbConnection);
        }


        // TODO might need to change the CartItem to Product once its implemented.
        public Order CreateOrderFromCart(CartItem[] items)
        {
            Order order = new Order(RandomOrderID(), _userName);
            foreach (CartItem item in items)
            {
                order.AddOrderItem(new OrderItem(item));
            }

            _orders.Add(order);
            if (_toSave)
            {
                _orderDL.AddOrder(order);
            }
            MarketLog.Log("OrderPool", "User " + _userName + " added new order from the cart.");
            return order;
        }

        public Order CreateOrder()
        {
            Order order = new Order(RandomOrderID(), _userName);

            _orders.Add(order);
            if (_toSave)
            {
                _orderDL.AddOrder(order);
            }
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
                    if (_toSave)
                    {
                        _orderDL.RemoveOrder(orderId);
                    }
                    MarketLog.Log("OrderPool", "User " + _userName + " removed order ID: "+orderId+" from his OrderPool");
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
                        double newPrice = order.GetPrice() - item.GetPrice();
                        order.RemoveOrderItem(item);
                        if (_toSave)
                        {
                            _orderDL.RemoveItemFromOrder(orderID, name, store);
                            _orderDL.UpdateOrderPrice(orderID, newPrice);
                        }
                        MarketLog.Log("OrderPool", "User " + _userName + " removed order Item from order ID: " + orderID);

                    }

                }
            }
        }

        public void AddItemToOrder(int orderID, OrderItem item)
        {
            var order = getOrder(orderID);
            order.AddOrderItem(item);
            double newPrice = order.GetPrice() + item.GetPrice();
            order.setPrice(newPrice);
            if (_toSave)
            {
                _orderDL.AddItemToOrder(orderID, item);
                _orderDL.UpdateOrderPrice(orderID,newPrice);
            }
            MarketLog.Log("OrderPool", "User " + _userName + " added order Item to order ID: " + orderID);

        }

        public int RandomOrderID()
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
            OrderItem order = _orderDL.FindOrderItemInOrder(orderId, store, user);
            if(order == null)
               throw new OrderException(0,"No order item found in order ID: "+orderId);
            return order;
        }


    }
}
