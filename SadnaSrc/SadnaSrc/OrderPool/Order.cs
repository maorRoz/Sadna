using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.OrderPool
{
    public class Order
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

        public void setPrice(double price) {_totalPrice = price;}
        public void setAddress(string address) { _shippingAddress = address; }

        public Order(int orderId, string userName)
        {
            _userName = userName;
            _shippingAddress = "default"; // TODO maybe change to other default value
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

        public Order(int orderId, string userName, string shippingAddress, double price, string dateString,
            List<OrderItem> items)
        {
            _userName = userName;
            _shippingAddress = shippingAddress;
            _items = items;
            _orderId = orderId;
            _totalPrice = price;
            _date = parseDate(dateString);
        }

        private DateTime parseDate(string dateString)
        {
            int days= Int32.Parse(dateString.Substring(0,2));
            int months = Int32.Parse(dateString.Substring(3, 2));
            int years = Int32.Parse(dateString.Substring(6, 4));
            return new DateTime(years,months,days);


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
                throw new OrderException(0, "Order item is already listed in the order.");
            }
            else
            {
                _items.Add(item);
                _totalPrice += (item.GetPrice()*item.GetQuantity());
            }
        }

        public void RemoveOrderItem(OrderItem item)
        {
            if (getOrderItem(item.GetName(), item.GetStore()) == null)
            {
                throw new OrderException(0, "Order item is not listed in the order.");
            }
            else
            {
                _items.Remove(item);
                _totalPrice -= (item.GetPrice() * item.GetQuantity());

            }
        }

        public void ComputeTotalPrice()
        {
            double acc = 0;
            foreach (OrderItem item in _items)
            {
                acc += item.GetPrice()*item.GetQuantity();
            }

            _totalPrice = acc;
        }

        public object[] ToData()
        {
            object[] ret = { _orderId, _userName, _shippingAddress, _totalPrice, _date.ToString("dd/MM/yyyy") };
            
            return ret;
        }

        public void SetShippingAddress(string shippingAddress)
        {
            _shippingAddress = shippingAddress;
        }
    }
}
