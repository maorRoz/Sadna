using SadnaSrc.UserSpot;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.StoreCenter;
using SadnaSrc.SupplyPoint;
using SadnaSrc.Walleter;

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


        public OrderService(UserService userService, StoreService storeService,
            SupplyService supplyService, PaymentService paymentService)
        {
            _orders= new List<Order>();
            User user = userService.GetUser();
            _userName = "Guest";
            if (user != null && user.IsRegisteredUser())
            {
                _userName = ((RegisteredUser) user).Name;
            }
            _orderDL = new OrderPoolDL();
        }
        
        public Order InitOrder(OrderItem[] items)
        {
            Order order = new Order(RandomOrderID(), _userName);
            foreach (OrderItem item in items)
            {
                order.AddOrderItem(item);
            }

            _orders.Add(order);

            return order;
        }

        public Order InitOrder()
        {
            Order order = new Order(RandomOrderID(), _userName);

            _orders.Add(order);


            return order;
        }

        public void SynchDB()
        {
            foreach (Order order in _orders)
            {
                _orderDL.AddOrder(order);

            }
        }

        public void RemoveOrderFromDB(int orderId)
        {
           _orderDL.RemoveOrder(orderId);
        }

        public Order getOrder(int orderID)
        {
            foreach (Order order in _orders)
            {
                if (order.GetOrderID() == orderID) return order;
            }

            return null;
        }

        public OrderItem FindOrderItemInOrder(int orderId, string store, string user)
        {
            foreach (Order order in _orders)
            {
                return order.getOrderItem(user, store);
            }

            return null;
        }

        /*
         * Interface functions
         */

        public MarketAnswer CreateOrder()
        {
            Order order = InitOrder();
            MarketLog.Log("OrderPool", "User " + _userName + " successfully initialized new order.");
            return new OrderAnswer(OrderStatus.Success, "Success, You created an order with ID: " + order.GetOrderID());

        }

        public MarketAnswer RemoveOrder(int orderId)
        {
            foreach (Order order in _orders)
            {
                if (order.GetOrderID() == orderId)
                {
                    _orders.Remove(order);
                    MarketLog.Log("OrderPool", "User " + _userName + " successfully removed order ID: " + orderId+" from his OrderPool");
                    return new OrderAnswer(OrderStatus.Success, "Success, You removed order Item from order ID: " + order.GetOrderID());
                }
            }
            throw new OrderException(OrderStatus.NoOrderWithID, "Failed, No Order with the specific ID.");
        }

        public MarketAnswer RemoveItemFromOrder(int orderID, string store, string name)
        {
            foreach (Order order in _orders)
            {
                if(order.GetOrderID() == orderID)
                {
                    var item = order.getOrderItem(name, store);
                    order.RemoveOrderItem(item);
                    MarketLog.Log("OrderPool", "User " + _userName + " successfully removed an order Item from order ID: " + orderID);
                    return new OrderAnswer(OrderStatus.Success, "Success, You removed an order Item from order ID: " + orderID);
                }
            }
            throw new OrderException(OrderStatus.NoOrderWithID, "Failed, No Order with the specific ID.");
        }

        public MarketAnswer AddItemToOrder(int orderID, OrderItem item)
        {
            var order = getOrder(orderID);
            order.AddOrderItem(item);
            MarketLog.Log("OrderPool", "User " + _userName + " successfully added an order Item to order ID: " + orderID);
            return new OrderAnswer(OrderItemStatus.Success, "Success, you added an order Item to order ID: " + orderID);
        }

        /*
         * Private Functions
         */

        private int RandomOrderID()
        {
            var ret = new Random().Next(100000, 999999);
            while (_orderDL.FindOrder(ret) != null)
            {
                ret = new Random().Next(100000, 999999);
            }

            return ret;
        }

       

        
    }
}
