using SadnaSrc.UserSpot;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.SupplyPoint;
using SadnaSrc.Walleter;

namespace SadnaSrc.OrderPool
{
    public class OrderService : IOrderService
    {
        public string UserName { get; set; }
        public string UserAddress { get; set; }
        public string CreditCard { get; set; }

        public List<Order> Orders;
        private readonly OrderPoolDL _orderDL;
        
        private readonly IUserBuyer _buyer;
        private IStoresSyncher _storesSync;

        private SupplyService _supplyService;
        private PaymentService _paymentService;


        //only for Unit Tests of developer!!(not for integration or blackbox or real usage)
        public void LoginBuyer(string userName,string password,string creditCard)
        {
            ((UserBuyerHarmony) _buyer).LogInBuyer(UserName, password);
            UserName = userName;
            UserAddress = _buyer.GetAddress();
        }

        //only for Unit Tests of developer!!(not for integration or blackbox or real usage)
        public void MakeGuest()
        {
            ((UserBuyerHarmony)_buyer).MakeGuest();
        }
        //TODO: Add payment and supply to the Ctor of ORderService
        //TODO: Add bootle credit card support (in tests too !!) until Maor finishes his branch (after that get credit card details from buyer)
        public OrderService(IUserBuyer buyer, IStoresSyncher storesSync)
        {
            Orders = new List<Order>();
            _buyer = buyer;
            _storesSync = storesSync;

            UserName = buyer.GetName();
            UserAddress = _buyer.GetAddress();
            _orderDL = new OrderPoolDL();


        }

        private void IsValidUserDetails()
        {
            if (UserName == null || UserAddress == null)
            {
                throw new OrderException(OrderStatus.InvalidNameOrAddress,"Cannot proceed with order if no valid user details has been given!");
            }
        }

        private void IsValidUserDetails(string userName, string address)
        {
            if (userName == null || address == null)
            {
                MarketLog.Log("OrderPool", "User entered name or address which is invalid by the system standards!");
                throw new  OrderException(GiveDetailsStatus.InvalidNameOrAddress, "User entered invalid name or address into the order");
            }
        }
        public MarketAnswer GiveDetails(string userName, string address)
        {
            MarketLog.Log("OrderPool","User entering name and address for later usage in market order. validating data ...");
            IsValidUserDetails(userName, address);
            MarketLog.Log("OrderPool", "Validation has been completed. User name and address are valid and been updated");
            UserName = userName;
            UserAddress = address;
            return new OrderAnswer(GiveDetailsStatus.Success,"User name and address has been updated successfully!");
        }
        
        public Order InitOrder(OrderItem[] items)
        {
            Order order = new Order(RandomOrderID(), UserName);
            foreach (OrderItem item in items)
            {
                order.AddOrderItem(item);
            }

            Orders.Add(order);
            MarketLog.Log("OrderPool", "A new order with items has been created ...");
            return order;
        }

        public Order InitOrder()
        {
            Order order = new Order(RandomOrderID(), UserName);
            Orders.Add(order);
            MarketLog.Log("OrderPool", "A new order has been created ...");

            return order;
        }

        public void SaveToDB()
        {
            foreach (Order order in Orders)
            {
                _orderDL.AddOrder(order);
            }
        }

        public void CleanSession()
        {
            foreach (Order order in Orders)
            {
                _orderDL.RemoveOrder(order.GetOrderID());
            }
            _buyer.CleanSession();
        }

        public void SaveOrderToDB(int orderId)
        {   
            _orderDL.AddOrder(GetOrder(orderId));
        }

        public void RemoveOrderFromDB(int orderId)
        {
            _orderDL.RemoveOrder(orderId);
        }      
        
        public Order GetOrder(int orderID)
        {
            foreach (Order order in Orders)
            {
                if (order.GetOrderID() == orderID) return order;
            }
            return null;
        }

        public Order GetOrderFromDB(int orderID)
        {
            return _orderDL.FindOrder(orderID);
        }

        public OrderItem FindOrderItemInOrder(int orderId, string store, string user)
        {
            foreach (Order order in Orders)
            {
                return order.GetOrderItem(user, store);
            }

            return null;
        }

        /*
         * Interface functions
         */
         //TODO: continue this
        public MarketAnswer BuyItemFromImmediate(string itemName, string store, int quantity, double unitPrice)
        {
            MarketLog.Log("OrderPool","Attempting to buy "+quantity +" "+itemName +" from store "+store+" in immediate sale...");
            int orderId = 0;
            try
            {
                IsValidUserDetails();
                OrderItem toBuy = _buyer.CheckoutItem(itemName, store, quantity, unitPrice);
                Order order = InitOrder();
                orderId = order.GetOrderID();
                order.AddOrderItem(toBuy);
                _supplyService.CreateDelivery(order);
                _paymentService.ProccesPayment(order, "");
                MarketLog.Log("OrderPool", "User " + UserName + " successfully initialized new order " + order.GetOrderID() + ".");
                return new OrderAnswer(OrderStatus.Success, "Successfully bought item "+itemName);

            }
            catch (OrderException e)
            {
                MarketLog.Log("OrderPool", "Order "+orderId+" has failed to execute. Error message has been created!");
                return new OrderAnswer((OrderStatus)e.Status, e.GetErrorMessage());
            }
            catch (WalleterException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with payment system." +
                                           " Error message has been created!");
                return new OrderAnswer(OrderStatus.NoPaymentConnection, e.GetErrorMessage());
            }
            catch (SupplyException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
                return new OrderAnswer(OrderStatus.NoSupplyConnection, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Something is wrong with Store or User." +
                                           " Error message has been created!");
                return new OrderAnswer(OrderStatus.InvalidUser, e.GetErrorMessage());
            }
        }
        //TODO: continue this
        public MarketAnswer BuyLotteryTicket(string itemName, string store, int quantity, double unitPrice)
        {
            MarketLog.Log("OrderPool", "Attempting to buy " + quantity + " tickets for lottery sale of " + itemName +
                                       " from store " + store + "...");
            int orderId = 0;
            try
            {
                IsValidUserDetails();
                OrderItem ticketToBuy = _buyer.CheckoutItem(itemName, store, quantity, unitPrice);
                Order order = InitOrder();
                orderId = order.GetOrderID();
            }
            catch (OrderException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Error message has been created!");
                return new OrderAnswer((OrderStatus) e.Status, e.GetErrorMessage());
            }
            catch (WalleterException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with payment system." +
                                           " Error message has been created!");
                return new OrderAnswer(OrderStatus.NoPaymentConnection, e.GetErrorMessage());
            }
            catch (SupplyException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
                return new OrderAnswer(OrderStatus.NoSupplyConnection, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Something is wrong with Store or User." +
                                           " Error message has been created!");
                return new OrderAnswer(OrderStatus.InvalidUser, e.GetErrorMessage());
            }
        }
        //TODO: continue this
        public MarketAnswer BuyAllItemsFromStore(string store)
        {
            MarketLog.Log("OrderPool", "Attempting to buy everything in cart from store " + store + "...");
            int orderId = 0;
            try
            {
                IsValidUserDetails();
                OrderItem[] itemsToBuy = _buyer.CheckoutFromStore(store);
                Order order = InitOrder();
                orderId = order.GetOrderID();
            }
            catch (OrderException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Error message has been created!");
                return new OrderAnswer((OrderStatus)e.Status, e.GetErrorMessage());
            }
            catch (WalleterException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with payment system." +
                                           " Error message has been created!");
                return new OrderAnswer(OrderStatus.NoPaymentConnection, e.GetErrorMessage());
            }
            catch (SupplyException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
                return new OrderAnswer(OrderStatus.NoSupplyConnection, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Something is wrong with Store or User." +
                                           " Error message has been created!");
                return new OrderAnswer(OrderStatus.InvalidUser, e.GetErrorMessage());
            }
        }
        //TODO: continue this
        public MarketAnswer BuyEverythingFromCart()
        {
            MarketLog.Log("OrderPool", "Attempting to buy everything in cart...");
            int orderId = 0;
            try
            {
                IsValidUserDetails();
                OrderItem[] itemsToBuy = _buyer.CheckoutAll();
                Order order = InitOrder();
                orderId = order.GetOrderID();
            }
            catch (OrderException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Error message has been created!");
                return new OrderAnswer((OrderStatus)e.Status, e.GetErrorMessage());
            }
            catch (WalleterException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with payment system." +
                                           " Error message has been created!");
                return new OrderAnswer(OrderStatus.NoPaymentConnection, e.GetErrorMessage());
            }
            catch (SupplyException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
                return new OrderAnswer(OrderStatus.NoSupplyConnection, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Something is wrong with Store or User." +
                                           " Error message has been created!");
                return new OrderAnswer(OrderStatus.InvalidUser, e.GetErrorMessage());
            }
        }
        //TODO: client can create order? why? why should he deal wih order id? 
        //TODO: (maybe the name of the method isnt right and confusing... but the id entering are wrong anyhow)
        public MarketAnswer CreateOrder(out int orderId)
        {
            Order order = InitOrder();
            orderId = order.GetOrderID();
            MarketLog.Log("OrderPool", "User " + UserName + " successfully initialized new order "+ order.GetOrderID()+".");
            return new OrderAnswer(OrderStatus.Success, "Success, You created an order with ID: " + order.GetOrderID());

        }
        //TODO: client can remove order? why? why should he deal wih order id?
        public MarketAnswer RemoveOrder(int orderId)
        {
            foreach (Order order in Orders)
            {
                if (order.GetOrderID() == orderId)
                {
                    Orders.Remove(order);
                    MarketLog.Log("OrderPool", "User " + UserName + " successfully removed order " + orderId+" from his OrderPool");
                    return new OrderAnswer(OrderStatus.Success, "Success, You removed order Item from order ID: " + order.GetOrderID());
                }
            }
            throw new OrderException(OrderStatus.NoOrderWithID, "Failed, No Order with the specific ID of "+ orderId+" .");
        }

        //TODO: you're giving the client too much power here, he had enough time to remove the item from his cart before he came here...
        //TODO: but its a great function in case you want the order to continue even if some item were badly placed here and got error. 
        //TODO: just not for client purpose
        public MarketAnswer RemoveItemFromOrder(int orderID, string store, string name)
        {
            foreach (Order order in Orders)
            {
                if(order.GetOrderID() == orderID)
                {
                    var item = order.GetOrderItem(name, store);
                    order.RemoveOrderItem(item);
                    MarketLog.Log("OrderPool", "User " + UserName + " successfully removed an order Item from order ID: " + orderID);
                    return new OrderAnswer(OrderStatus.Success, "Success, You removed an order Item from order ID: " + orderID);
                }
            }
            throw new OrderException(OrderStatus.NoOrderWithID, "Failed, No Order with the specific ID.");
        }
        //TODO: you should replace this with single buy option. it should result in actual buy and not in some kind of a second storage unit 
        //TODO: like cart in UserSpot
        public MarketAnswer AddItemToOrder(int orderID, string store, string name, double price, int quantity)
        {
            var order = GetOrder(orderID);
            order.AddOrderItem(new OrderItem(store, name, price, quantity));
            MarketLog.Log("OrderPool", "User " + UserName + " successfully added an order Item to order ID: " + orderID);
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
