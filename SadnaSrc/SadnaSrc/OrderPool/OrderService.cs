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

        private static Random rand = new Random();


        public List<Order> Orders;
        private readonly OrderPoolDL _orderDL;
        
        private readonly IUserBuyer _buyer;
        private IStoresSyncher _storesSync;

        private SupplyService _supplyService;
        private PaymentService _paymentService;


        //only for Unit Tests of developer!!(not for integration or blackbox or real usage)
        public void LoginBuyer(string userName,string password)
        {
            ((UserBuyerHarmony) _buyer).LogInBuyer(UserName, password);
            UserName = userName;
            UserAddress = _buyer.GetAddress();
            CreditCard = _buyer.GetCreditCard();
        }

        //only for Unit Tests of developer!!(not for integration or blackbox or real usage)
        public void MakeGuest()
        {
            ((UserBuyerHarmony)_buyer).MakeGuest();
        }
        //TODO: Add bootle credit card support (in tests too !!) until Maor finishes his branch (after that get credit card details from buyer)
        public OrderService(IUserBuyer buyer, IStoresSyncher storesSync, PaymentService paymentService, SupplyService supplyService)
        {
            Orders = new List<Order>();
            _buyer = buyer;
            _storesSync = storesSync;
            _supplyService = supplyService;
            _paymentService = paymentService;
            UserName = buyer.GetName();
            UserAddress = _buyer.GetAddress();
            CreditCard = _buyer.GetCreditCard();
            _orderDL = new OrderPoolDL();

        }

        
        public MarketAnswer GiveDetails(string userName, string address, string creditCard)
        {
            MarketLog.Log("OrderPool","User entering name and address for later usage in market order. validating data ...");
            IsValidUserDetails(userName, address, creditCard);
            MarketLog.Log("OrderPool", "Validation has been completed. User name and address are valid and been updated");
            UserName = userName;
            UserAddress = address;
            CreditCard = creditCard;
            return new OrderAnswer(GiveDetailsStatus.Success,"User name and address has been updated successfully!");
        }
        
        public Order InitOrder(OrderItem[] items)
        {
            CheckAllItems(items);
            Order order = new Order(RandomOrderID(), UserName, UserAddress);
            foreach (OrderItem item in items)
            {
                order.AddOrderItem(item);
            }

            Orders.Add(order);
            MarketLog.Log("OrderPool", "User " + UserName + " successfully initialized new order " + order.GetOrderID() + ".");
            return order;
        }

        public Order InitOrder()
        {
            Order order = new Order(RandomOrderID(), UserName, UserAddress);
            Orders.Add(order);
            MarketLog.Log("OrderPool", "User " + UserName + " successfully initialized new order " + order.GetOrderID() + ".");
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

        public void SaveOrderToDB(Order order)
        {   
            _orderDL.AddOrder(order);
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

        public OrderItem FindOrderItemInOrder(int orderId, string store, string name)
        {
            foreach (Order order in Orders)
            {
                return order.GetOrderItem(name, store);
            }

            return null;
        }

        /*
         * Interface functions
         */

        public MarketAnswer BuyItemFromImmediate(string itemName, string store, int quantity, double unitPrice)
        {
            MarketLog.Log("OrderPool","Attempting to buy "+quantity +" "+itemName +" from store "+store+" in immediate sale...");
            int orderId = 0;
            try
            {
                IsValidUserDetails();
                OrderItem toBuy = _buyer.CheckoutItem(itemName, store, quantity, unitPrice);
                CheckOrderItem(toBuy);
                Order order = InitOrder();
                orderId = order.GetOrderID();
                order.AddOrderItem(toBuy);
                _supplyService.CreateDelivery(order);
                _paymentService.ProccesPayment(order, CreditCard);
                SaveOrderToDB(order);
                OrderItem[] wrap = {toBuy};
                //_storesSync.RemoveProducts(wrap);
                MarketLog.Log("OrderPool", "User " + UserName + " successfully bought item "+ itemName + "in an immediate sale.");
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
                return new OrderAnswer((WalleterStatus)e.Status, e.GetErrorMessage());
            }
            catch (SupplyException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
                return new OrderAnswer((SupplyStatus)e.Status, e.GetErrorMessage());
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
                CheckOrderItem(ticketToBuy);
                Order order = InitOrder();
                orderId = order.GetOrderID();
                order.AddOrderItem(ticketToBuy);
                SaveOrderToDB(order);
                OrderItem[] wrap = { ticketToBuy };
                MarketLog.Log("OrderPool", "User " + UserName + " successfully bought lottery ticket.");
                return new OrderAnswer(OrderStatus.Success, "Successfully bought Lottery ticket ");

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
                return new OrderAnswer((WalleterStatus)e.Status, e.GetErrorMessage());
            }
            catch (SupplyException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
                return new OrderAnswer((SupplyStatus)e.Status, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Something is wrong with Store or User." +
                                           " Error message has been created!");
                return new OrderAnswer(OrderStatus.InvalidUser, e.GetErrorMessage());
            }
        }


        public MarketAnswer BuyAllItemsFromStore(string store)
        {
            MarketLog.Log("OrderPool", "Attempting to buy everything in cart from store " + store + "...");
            int orderId = 0;
            try
            {
                IsValidUserDetails();
                OrderItem[] itemsToBuy = _buyer.CheckoutFromStore(store);
                Order order = InitOrder(itemsToBuy);                
                _supplyService.CreateDelivery(order);
                _paymentService.ProccesPayment(order, CreditCard);
                SaveOrderToDB(order);
                //_storesSync.RemoveProducts(itemsToBuy);
                MarketLog.Log("OrderPool", "User " + UserName + " successfully bought all the items in store :"+store+".");
                return new OrderAnswer(OrderStatus.Success, "Successfully bought all the items in store :" + store + ".");
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
                return new OrderAnswer((WalleterStatus)e.Status, e.GetErrorMessage());
            }
            catch (SupplyException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
                return new OrderAnswer((SupplyStatus)e.Status, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Something is wrong with Store or User." +
                                           " Error message has been created!");
                return new OrderAnswer(OrderStatus.InvalidUser, e.GetErrorMessage());
            }
        }


        public MarketAnswer BuyEverythingFromCart()
        {
            MarketLog.Log("OrderPool", "Attempting to buy everything in cart...");
            int orderId = 0;
            try
            {
                IsValidUserDetails();
                OrderItem[] itemsToBuy = _buyer.CheckoutAll();
                Order order = InitOrder(itemsToBuy);
                _supplyService.CreateDelivery(order);
                _paymentService.ProccesPayment(order, CreditCard);
                SaveOrderToDB(order);
                //_storesSync.RemoveProducts(itemsToBuy);
                MarketLog.Log("OrderPool", "User " + UserName + " successfully bought all the items in the cart.");
                return new OrderAnswer(OrderStatus.Success, "Successfully bought all the items in the cart.");
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
                return new OrderAnswer((WalleterStatus)e.Status, e.GetErrorMessage());
            }
            catch (SupplyException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
                return new OrderAnswer((SupplyStatus)e.Status, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Something is wrong with Store or User." +
                                           " Error message has been created!");
                return new OrderAnswer(OrderStatus.InvalidUser, e.GetErrorMessage());
            }
        }

        public MarketAnswer Refund(double sum)
        {
            MarketLog.Log("OrderPool", "Attempting to refund...");
            int orderId = 0;
            try
            {
                IsValidUserDetails();
                Order order = RefundOrder(sum);
                _paymentService.Refund(sum, CreditCard,UserName);
                SaveOrderToDB(order);
                MarketLog.Log("OrderPool", "User " + UserName + " successfully refunded the sum: "+sum);
                return new OrderAnswer(OrderStatus.Success, "Successfully refunded the sum: " + sum);
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
                return new OrderAnswer((WalleterStatus)e.Status, e.GetErrorMessage());
            }
            catch (SupplyException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
                return new OrderAnswer((SupplyStatus)e.Status, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Something is wrong with Store or User." +
                                           " Error message has been created!");
                return new OrderAnswer(OrderStatus.InvalidUser, e.GetErrorMessage());
            }
        }

        public MarketAnswer SendPackage(string itemName, string store, int quantity)
        {
            MarketLog.Log("OrderPool", "Attempting to send package...");
            int orderId = 0;
            try
            {
                IsValidUserDetails();
                OrderItem toBuy = _buyer.CheckoutItem("DELIVERY : "+itemName, store, quantity, 1);
                CheckOrderItem(toBuy);
                Order order = InitOrder();
                orderId = order.GetOrderID();
                order.AddOrderItem(toBuy);
                _supplyService.CreateDelivery(order);
                SaveOrderToDB(order);
                MarketLog.Log("OrderPool", "User " + UserName + " successfully made delivery for item: " + itemName + " X "+quantity);
                return new OrderAnswer(OrderStatus.Success, "Successfully made delivery for item: " + itemName + " X " + quantity);
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
                return new OrderAnswer((WalleterStatus)e.Status, e.GetErrorMessage());
            }
            catch (SupplyException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
                return new OrderAnswer((SupplyStatus)e.Status, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Something is wrong with Store or User." +
                                           " Error message has been created!");
                return new OrderAnswer(OrderStatus.InvalidUser, e.GetErrorMessage());
            }
        }
        /*
         * Private Functions
         */

        private int RandomOrderID()
        {
            var ret = rand.Next(100000, 999999);
            while (_orderDL.FindOrder(ret) != null)
            {
                ret = rand.Next(100000, 999999);
            }

            return ret;
        }

        private Order RefundOrder(double sum)
        {
            Order refund = new Order(RandomOrderID(),UserName,UserAddress);
            refund.AddOrderItem(new OrderItem("","Refund", -1 * sum,1));
            return refund;
        }

        private void IsValidUserDetails()
        {
            if (UserName == null || UserAddress == null || CreditCard == null)
            {
                throw new OrderException(OrderStatus.InvalidNameOrAddress, "Cannot proceed with order if no valid user details has been given!");
            }
        }

        private void IsValidUserDetails(string userName, string address, string creditCard)
        {
            if (userName == null || address == null || creditCard == null)
            {
                MarketLog.Log("OrderPool", "User entered name or address which is invalid by the system standards!");
                throw new OrderException(GiveDetailsStatus.InvalidNameOrAddress, "User entered invalid name or address into the order");
            }
        }

        private void CheckOrderItem(OrderItem item)
        {
            if (item.Name == null || item.Store == null || item.Quantity == 0)
            {
                MarketLog.Log("OrderPool", "User entered item details which are invalid by the system standards!");
                throw new OrderException(OrderItemStatus.InvalidDetails, "User entered invalid item details");
            }
        }

        private void CheckAllItems(OrderItem[] items)
        {
            if (items.Length == 0)
            {
                MarketLog.Log("OrderPool", "User entered empty item list !");
                throw new OrderException(OrderItemStatus.InvalidDetails, "User entered empty item list");
            }
            foreach (var item in items)
            {
                CheckOrderItem(item);
            }
        }
    }
}
