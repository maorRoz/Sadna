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
        private string _userName;
        private readonly OrderPoolDL _orderDL;
        private List<Order> _orders;
        private bool _toSave;


        public OrderService(string userName, bool toSave, SQLiteConnection dbConnection)
        {
            _orders= new List<Order>();
            _userName = userName;
            _toSave = toSave;
            _orderDL = new OrderPoolDL(dbConnection);
        }

        public void GetOrderFromCart(CartItem[] items)
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
