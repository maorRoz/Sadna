using SadnaSrc.UserSpot;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.OrderPool
{
    class OrderService
    {
        // TODO add support to external systems once there is more clarification about the systems. !!! IMPORTANT !!!
        private string _userName;
        private readonly OrderPoolDL _orderDL;
        private List<Order> _orders;
        private bool _toSave;
        private string _shippingAddress;

        public OrderService(string userName, bool toSave, SQLiteConnection dbConnection)
        {
            _orders= new List<Order>();
            _userName = userName;
            _toSave = toSave;
            _orderDL = new OrderPoolDL(dbConnection);
        }

        // TODO might need to change the CartItem to Product once its implemented.
        public void CreateOrderFromCart(CartItem[] items)
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
        }

        public void CreateOrder(OrderItem[] items)
        {
            Order order = new Order(RandomOrderID(), _userName);
            foreach (OrderItem item in items)
            {
                order.AddOrderItem(item);
            }

            _orders.Add(order);
            if (_toSave)
            {
                _orderDL.AddOrder(order);
            }
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
                }
            } 
        }

        public void RemoveItemFromOrder(int orderID, string store, string name)
        {
            foreach (Order order in _orders)
            {
                if(order.GetOrderID() == orderID)
                {
                    order.RemoveOrderItem(order.getOrderItem(name,store));
                    if (_toSave)
                    {
                        _orderDL.RemoveItemFromOrder(orderID,name,store);
                    }
                }
            }

        }

        public int RandomOrderID()
        {
            int ret = new Random().Next(100000, 999999);
            while (_orderDL.FindOrder(ret) != null)
            {
                ret = new Random().Next(100000, 999999);
            }

            return ret;
        }


    }
}
