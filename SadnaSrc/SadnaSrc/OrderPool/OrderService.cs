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

        public static Random randy = new Random();


        public List<Order> Orders;
        private readonly OrderPoolDL _orderDL;
        
        private readonly IUserBuyer _buyer;
        private IStoresSyncher _storesSync;

        private SupplyService _supplyService;
        private PaymentService _paymentService;



        public OrderService(IUserBuyer buyer, IStoresSyncher storesSync)
        {
            Orders = new List<Order>();
            _buyer = buyer;
            _storesSync = storesSync;
            _supplyService = SupplyService.Instance;
            _paymentService = PaymentService.Instance;
            GetUserDetailsFromBuyer();
            _orderDL = new OrderPoolDL();

            _supplyService.AttachExternalSystem();
            _paymentService.AttachExternalSystem();

        }

        public OrderService(IStoresSyncher storesSync, PaymentService paymentService)
        {
            _storesSync = storesSync;
            _paymentService = paymentService;
            _orderDL = new OrderPoolDL();

        }

        public static void CheckOrderItem(OrderItem item)
        {
            if (item.Name == null || item.Store == null || item.Quantity == 0)
            {
                MarketLog.Log("OrderPool", "User entered item details which are invalid by the system standards!");
                throw new OrderException(OrderItemStatus.InvalidDetails, "User entered invalid item details");
            }
        }

        //only for Unit Tests of developer!!(not for integration or blackbox or real usage)
        public void LoginBuyer(string userName, string password)
        {
            ((UserBuyerHarmony)_buyer).LogInBuyer(UserName, password);
            GetUserDetailsFromBuyer();
        }

        //only for Unit Tests of developer!!(not for integration or blackbox or real usage)
        public void MakeGuest(string userName, string address, string creditCard)
        {
            ((UserBuyerHarmony)_buyer).MakeGuest();
            GiveDetails(userName, address, creditCard);
        }

        public Order InitOrder(OrderItem[] items)
        {
            GetUserDetailsFromBuyer();
            CheckAllItems(items);
            Order order = new Order(_orderDL.RandomOrderID(), UserName, UserAddress);
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
            GetUserDetailsFromBuyer();
            Order order = new Order(_orderDL.RandomOrderID(), UserName, UserAddress);
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
                OrderItem toBuy = _buyer.CheckoutItem(itemName, store, quantity, unitPrice);
                CheckOrderItem(toBuy);
                Order order = InitOrder();
                orderId = order.GetOrderID();
                order.AddOrderItem(toBuy);
                _supplyService.CreateDelivery(order);
                _paymentService.ProccesPayment(order, CreditCard);
                SaveOrderToDB(order);
                OrderItem[] wrap = {toBuy};
                _storesSync.RemoveProducts(wrap);
                _buyer.RemoveItemFromCart(itemName, store, quantity, unitPrice);
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

        public MarketAnswer BuyItemWithCoupon(string itemName, string store, int quantity, string coupon)
        {
            MarketLog.Log("OrderPool", "Attempting to buy " + quantity + " " + itemName + " from store " + store + " in immediate sale...");
            int orderId = 0;
            try
            {
                OrderItem toBuy = _storesSync.GetItemFromCoupon(itemName, store, quantity, coupon);
                if (toBuy == null)
                    throw new OrderException(OrderStatus.InvalidCoupon,
                        "Order has failed to execute. Invalid coupon number!");
                CheckOrderItem(toBuy);
                Order order = InitOrder();
                orderId = order.GetOrderID();
                order.AddOrderItem(toBuy);
                _supplyService.CreateDelivery(order);
                _paymentService.ProccesPayment(order, CreditCard);
                SaveOrderToDB(order);
                OrderItem[] wrap = { toBuy };
                _storesSync.RemoveProducts(wrap);
                MarketLog.Log("OrderPool", "User " + UserName + " successfully bought item " + itemName + "in an immediate sale.");
                return new OrderAnswer(OrderStatus.Success, "Successfully bought item " + itemName);

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

        public MarketAnswer BuyLotteryTicket(string itemName, string store, int quantity, double unitPrice)
        {
            MarketLog.Log("OrderPool", "Attempting to buy " + quantity + " tickets for lottery sale of " + itemName +
                                       " from store " + store + "...");
            int orderId = 0;
            try
            {
                OrderItem ticketToBuy = _buyer.CheckoutItem("LOTTERY: "+itemName, store, quantity, unitPrice);
                CheckOrderItem(ticketToBuy);
                Order order = InitOrder();
                orderId = order.GetOrderID();
                order.AddOrderItem(ticketToBuy);
                _paymentService.ProccesPayment(order,CreditCard);
                SaveOrderToDB(order);
                OrderItem[] wrap = { ticketToBuy };
                // TODO: maybe add function here to notify the store of the successful purchase.
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
                OrderItem[] itemsToBuy = _buyer.CheckoutFromStore(store);
                Order order = InitOrder(itemsToBuy);                
                _supplyService.CreateDelivery(order);
                _paymentService.ProccesPayment(order, CreditCard);
                SaveOrderToDB(order);
                _storesSync.RemoveProducts(itemsToBuy);
                _buyer.EmptyCart(store);
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
                OrderItem[] itemsToBuy = _buyer.CheckoutAll();
                Order order = InitOrder(itemsToBuy);
                _supplyService.CreateDelivery(order);
                _paymentService.ProccesPayment(order, CreditCard);
                SaveOrderToDB(order);
                _storesSync.RemoveProducts(itemsToBuy);
                _buyer.EmptyCart();
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
        

        public MarketAnswer GiveDetails(string userName, string address, string creditCard)
        {
            MarketLog.Log("OrderPool", "User entering name and address for later usage in market order. validating data ...");
            IsValidUserDetails(userName, address, creditCard);
            MarketLog.Log("OrderPool", "Validation has been completed. User name and address are valid and been updated");
            UserName = userName;
            UserAddress = address;
            CreditCard = creditCard;
            return new OrderAnswer(GiveDetailsStatus.Success, "User name and address has been updated successfully!");
        }
        /*
         * Private Functions
         */

        private void IsValidUserDetails()
        {
            if (UserName == null || UserAddress == null || CreditCard == null)
            {
                throw new OrderException(OrderStatus.InvalidNameOrAddress, "Cannot proceed with order if no valid user details has been given!");
            }
        }

        private void GetUserDetailsFromBuyer()
        {
            UserName = _buyer.GetName();
            UserAddress = _buyer.GetAddress();
            CreditCard = _buyer.GetCreditCard();
        }

        private void IsValidUserDetails(string userName, string address, string creditCard)
        {
            int x;
            if (userName == null || address == null || creditCard == null || creditCard.Length != 8 || !Int32.TryParse(creditCard, out x))
            {
                MarketLog.Log("OrderPool", "User entered name or address which is invalid by the system standards!");
                throw new OrderException(GiveDetailsStatus.InvalidNameOrAddress, "User entered invalid name or address into the order");
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
