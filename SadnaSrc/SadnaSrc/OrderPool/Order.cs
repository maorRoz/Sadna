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
        private DateTime _date;

        public List<OrderItem> GetItems() { return _items; }
        public double GetPrice() { return _totalPrice; }
        public string GetShippingAddress() { return _shippingAddress; }
        public string GetUserName() { return _userName; }
        public int GetOrderID() { return _orderId; }
        public DateTime getDate() {  return _date;}

        public Order(int orderId, string userName)
        {
            _userName = userName;
            _shippingAddress = ""; // TODO maybe change to other default value
            _items = new List<OrderItem>();
            _orderId = orderId;
            _date = DateTime.Today;
        }

        public Order(int orderId, string userName, string shippingAddress)
        {
            _userName = userName;
            _shippingAddress = shippingAddress;
            _items = new List<OrderItem>();
            //_orderId = new Random().Next(100000, 999999);
            _orderId = orderId;
            _date = DateTime.Today;
        }

        public OrderItem getOrderItem(string name,string store)
        {
            foreach (OrderItem item in _items)
            {
                if (item.GetName() == name && item.GetStore() == store)
                    return item;
            }

            return null;
        }

        public void AddOrderItem(OrderItem item)
        {
            if (getOrderItem(item.GetName(), item.GetStore()) != null)
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
            if (getOrderItem(item.GetName(), item.GetStore()) == null)
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

        public object[] ToData()
        {
            object[] ret = { _orderId, _userName, _shippingAddress, _totalPrice, _date.ToString() };
            
            return ret;
        }

        public void SetShippingAddress(string shippingAddress)
        {
            _shippingAddress = shippingAddress;
        }
    }
}
