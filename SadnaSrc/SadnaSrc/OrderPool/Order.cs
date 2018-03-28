using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.OrderPool
{
    class Order
    {
        private List<OrderItem> _items;
        private double _totalPrice;
        private string _shippingAddress;
        private string _userName;
        private int _orderId;

        public List<OrderItem> GetItems() { return _items; }
        public double GetPrice() { return _totalPrice; }
        public string GetShippingAddress() { return _shippingAddress; }
        public string GetUserName() { return _userName; }
        public int GetOrderID() { return _orderId; }

        public Order(int orderId, string userName)
        {
            _userName = userName;
            _shippingAddress = ""; // TODO maybe change to other default value
            _items = new List<OrderItem>();
            _orderId = orderId;
        }

        public Order(int orderId, string userName, string shippingAddress)
        {
            _userName = userName;
            _shippingAddress = shippingAddress;
            _items = new List<OrderItem>();
            //_orderId = new Random().Next(100000, 999999);
            _orderId = orderId;
        }

        public bool CheckOrderItem(string name,string store)
        {
            foreach (OrderItem item in _items)
            {
                if (item.GetName() == name && item.GetStore() == store)
                    return true;
            }

            return false;
        }

        public void AddOrderItem(OrderItem item)
        {
            if (CheckOrderItem(item.GetName(), item.GetStore()))
            {
                throw new OrderException("Order item is already listed in the order.");
            }
            else
            {
                _items.Add(item);
                _totalPrice += item.GetPrice();
            }
        }

        public void RemoveOrderItem(OrderItem item)
        {
            if (!CheckOrderItem(item.GetName(), item.GetStore()))
            {
                throw new OrderException("Order item is not listed in the order.");
            }
            else
            {
                _items.Remove(item);
                _totalPrice -= item.GetPrice();

            }
        }

        public void ComputeTotalPrice()
        {
            double acc = 0;
            foreach (OrderItem item in _items)
            {
                acc += item.GetPrice();
            }

            _totalPrice = acc;
        }

    }
}
