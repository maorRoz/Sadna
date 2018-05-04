using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

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
        public DateTime GetDate() {  return _date;}

        public void SetPrice(double price) {_totalPrice = price;}
        public void SetAddress(string address) { _shippingAddress = address; }

        public Order(int orderId, string userName)
        {
            _userName = userName;
            _shippingAddress = "default"; 
            _items = new List<OrderItem>();
            _orderId = orderId;
            _date = DateTime.Now;
        }

        public Order(int orderId, string userName, string shippingAddress)
        {
            _userName = userName;
            _shippingAddress = shippingAddress;
            _items = new List<OrderItem>();
            _orderId = orderId;
            _date = DateTime.Now;
        }

        public Order(int orderId, string userName, string shippingAddress, double price, string dateString,
            List<OrderItem> items)
        {
            _userName = userName;
            _shippingAddress = shippingAddress;
            _items = items;
            _orderId = orderId;
            _totalPrice = price;
            _date = ParseDate(dateString);
        }

        private DateTime ParseDate(string dateString)
        {

            int days= Int32.Parse(dateString.Substring(0,2));
            int months = Int32.Parse(dateString.Substring(3, 2));
            int years = Int32.Parse(dateString.Substring(6, 4));
            return new DateTime(years,months,days);


        }

        public OrderItem GetOrderItem(string name,string store)
        {
            foreach (OrderItem item in _items)
            {
                if (item.Name == name && item.Store == store)
                    return item;
            }

            return null;
        }

        public void AddOrderItem(OrderItem item)
        {
            if (GetOrderItem(item.Name, item.Store) != null)
            {
                throw new OrderException(OrderItemStatus.ItemAlreadyInOrder, "Order item is already listed in the order.");
            }

            _items.Add(item);
            _totalPrice += item.Price*item.Quantity;
        }

        public void ComputeTotalPrice()
        {
            double acc = 0;
            foreach (OrderItem item in _items)
            {
                acc += item.Price*item.Quantity;
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
