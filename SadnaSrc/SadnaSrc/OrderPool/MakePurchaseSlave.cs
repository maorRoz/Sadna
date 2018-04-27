using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.SupplyPoint;
using SadnaSrc.Walleter;

namespace SadnaSrc.OrderPool
{
    public class MakePurchaseSlave
    {
        private string UserName { get; set; }
        private string UserAddress { get; set; }
        private string CreditCard { get; set; }


        private readonly OrderDL _orderDL;

        private readonly IUserBuyer _buyer;
        private readonly IStoresSyncher _storesSync;

        private readonly SupplyService _supplyService;
        private readonly PaymentService _paymentService;

        private int cheatCode = -1;

        public OrderAnswer Answer { get; private set; }

        public MakePurchaseSlave(IUserBuyer buyer, IStoresSyncher storesSync)
        {
            _buyer = buyer;
            _storesSync = storesSync;
            _supplyService = SupplyService.Instance;
            _paymentService = PaymentService.Instance;
            GetUserDetailsFromBuyer();
            _orderDL = OrderDL.Instance;

            _supplyService.AttachExternalSystem();
            _paymentService.AttachExternalSystem();

        }

        public Order BuyItemFromImmediate(string itemName, string store, int quantity, double unitPrice)
        {
            MarketLog.Log("OrderPool", "Attempting to buy " + quantity + " " + itemName + " from store " + store + " in immediate sale...");
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
                _orderDL.AddOrder(order);
                OrderItem[] wrap = { toBuy };
                _storesSync.RemoveProducts(wrap);
                _buyer.RemoveItemFromCart(itemName, store, quantity, unitPrice);
                MarketLog.Log("OrderPool", "User " + UserName + " successfully bought item " + itemName + "in an immediate sale.");
                Answer =  new OrderAnswer(OrderStatus.Success, "Successfully bought item " + itemName);
                return order;
            }
            catch (OrderException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Error message has been created!");
                Answer = new OrderAnswer((OrderStatus)e.Status, e.GetErrorMessage());
                return null;
            }
            catch (WalleterException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with payment system." +
                                           " Error message has been created!");
                Answer = new OrderAnswer((WalleterStatus)e.Status, e.GetErrorMessage());
                return null;
            }
            catch (SupplyException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
                Answer = new OrderAnswer((SupplyStatus)e.Status, e.GetErrorMessage());
                return null;
            }
            catch (MarketException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Something is wrong with Store or User." +
                                           " Error message has been created!");
                Answer = new OrderAnswer(OrderStatus.InvalidUser, e.GetErrorMessage());
                return null;
            }
        }

        public Order BuyItemWithCoupon(string itemName, string store, int quantity, double unitPrice, string coupon)
        {
            MarketLog.Log("OrderPool", "Attempting to buy " + quantity + " " + itemName + " from store " + store + " in immediate sale...");
            int orderId = 0;
            try
            {
                OrderItem toBuy = _buyer.CheckoutItem(itemName, store, quantity, unitPrice);
                try
                {
                    toBuy.Price = _storesSync.GetPriceFromCoupon(itemName, store, quantity, coupon);
                }
                catch (MarketException e)
                {
                    MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Something is wrong with Store." +
                                               " Error message has been created!");
                    Answer = new OrderAnswer(OrderStatus.InvalidCoupon, e.GetErrorMessage());
                    return null;
                }
                CheckOrderItem(toBuy);
                Order order = InitOrder();
                orderId = order.GetOrderID();
                order.AddOrderItem(toBuy);
                _supplyService.CreateDelivery(order);
                _paymentService.ProccesPayment(order, CreditCard);
                _orderDL.AddOrder(order);
                OrderItem[] wrap = { toBuy };
                _storesSync.RemoveProducts(wrap);
                MarketLog.Log("OrderPool", "User " + UserName + " successfully bought item " + itemName + "in an immediate sale.");
                Answer = new OrderAnswer(OrderStatus.Success, "Successfully bought item " + itemName);
                return order;

            }
            catch (OrderException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Error message has been created!");
                Answer = new OrderAnswer((OrderStatus)e.Status, e.GetErrorMessage());
                return null;
            }
            catch (WalleterException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with payment system." +
                                           " Error message has been created!");
                Answer = new OrderAnswer((WalleterStatus)e.Status, e.GetErrorMessage());
                return null;
            }
            catch (SupplyException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
                Answer = new OrderAnswer((SupplyStatus)e.Status, e.GetErrorMessage());
                return null;
            }
            catch (MarketException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Something is wrong with User." +
                                           " Error message has been created!");
                Answer = new OrderAnswer(OrderStatus.InvalidUser, e.GetErrorMessage());
                return null;
            }
        }

        public Order BuyAllItemsFromStore(string store)
        {
            MarketLog.Log("OrderPool", "Attempting to buy everything in cart from store " + store + "...");
            int orderId = 0;
            try
            {
                OrderItem[] itemsToBuy = _buyer.CheckoutFromStore(store);
                Order order = InitOrder(itemsToBuy);
                _supplyService.CreateDelivery(order);
                _paymentService.ProccesPayment(order, CreditCard);
                _orderDL.AddOrder(order);
                _storesSync.RemoveProducts(itemsToBuy);
                _buyer.EmptyCart(store);
                MarketLog.Log("OrderPool", "User " + UserName + " successfully bought all the items in store :" + store + ".");
                Answer = new OrderAnswer(OrderStatus.Success, "Successfully bought all the items in store :" + store + ".");
                return order;
            }
            catch (OrderException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Error message has been created!");
                Answer = new OrderAnswer((OrderStatus)e.Status, e.GetErrorMessage());
                return null;
            }
            catch (WalleterException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with payment system." +
                                           " Error message has been created!");
                Answer = new OrderAnswer((WalleterStatus)e.Status, e.GetErrorMessage());
                return null;
            }
            catch (SupplyException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
                Answer = new OrderAnswer((SupplyStatus)e.Status, e.GetErrorMessage());
                return null;
            }
            catch (MarketException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Something is wrong with Store or User." +
                                           " Error message has been created!");
                Answer = new OrderAnswer(OrderStatus.InvalidUser, e.GetErrorMessage());
                return null;
            }
        }

        public Order BuyEverythingFromCart()
        {
            MarketLog.Log("OrderPool", "Attempting to buy everything in cart...");
            int orderId = 0;
            try
            {
                OrderItem[] itemsToBuy = _buyer.CheckoutAll();
                Order order = InitOrder(itemsToBuy);
                _supplyService.CreateDelivery(order);
                _paymentService.ProccesPayment(order, CreditCard);
                _orderDL.AddOrder(order);
                _storesSync.RemoveProducts(itemsToBuy);
                _buyer.EmptyCart();
                MarketLog.Log("OrderPool", "User " + UserName + " successfully bought all the items in the cart.");
                Answer = new OrderAnswer(OrderStatus.Success, "Successfully bought all the items in the cart.");
                return order;
            }
            catch (OrderException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Error message has been created!");
                Answer = new OrderAnswer((OrderStatus)e.Status, e.GetErrorMessage());
                return null;
            }
            catch (WalleterException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with payment system." +
                                           " Error message has been created!");
                Answer = new OrderAnswer((WalleterStatus)e.Status, e.GetErrorMessage());
                return null;
            }
            catch (SupplyException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
                Answer = new OrderAnswer((SupplyStatus)e.Status, e.GetErrorMessage());
                return null;
            }
            catch (MarketException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Something is wrong with Store or User." +
                                           " Error message has been created!");
                Answer = new OrderAnswer(OrderStatus.InvalidUser, e.GetErrorMessage());
                return null;
            }
        }

        public Order BuyLotteryTicket(string itemName, string store, int quantity, double unitPrice)
        {
            MarketLog.Log("OrderPool", "Attempting to buy " + quantity + " tickets for lottery sale of " + itemName +
                                       " from store " + store + "...");
            int orderId = 0;
            try
            {
                _buyer.ValidateRegisteredUser();
                _storesSync.ValidateTicket(itemName, store, unitPrice);
                OrderItem ticketToBuy = new OrderItem(store, itemName, unitPrice, quantity);
                CheckOrderItem(ticketToBuy);
                Order order = InitOrder();
                orderId = order.GetOrderID();
                order.AddOrderItem(ticketToBuy);
                _paymentService.ProccesPayment(order, CreditCard);
                _orderDL.AddOrder(order, "Lottery");
                _storesSync.UpdateLottery(itemName, store, unitPrice, UserName, cheatCode);
                MarketLog.Log("OrderPool", "User " + UserName + " successfully bought lottery ticket.");
                Answer = new OrderAnswer(OrderStatus.Success, "Successfully bought Lottery ticket ");
                return order;
            }
            catch (OrderException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Error message has been created!");
                Answer = new OrderAnswer((OrderStatus)e.Status, e.GetErrorMessage());
                return null;
            }
            catch (WalleterException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with payment system." +
                                           " Error message has been created!");
                Answer = new OrderAnswer((WalleterStatus)e.Status, e.GetErrorMessage());
                return null;
            }
            catch (SupplyException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
                Answer = new OrderAnswer((SupplyStatus)e.Status, e.GetErrorMessage());
                return null;
            }
            catch (MarketException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Something is wrong with Store or User." +
                                           " Error message has been created!");
                Answer = new OrderAnswer(OrderStatus.InvalidUser, e.GetErrorMessage());
                return null;
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

        private Order InitOrder()
        {
            GetUserDetailsFromBuyer();
            Order order = new Order(_orderDL.RandomOrderID(), UserName, UserAddress);
            MarketLog.Log("OrderPool", "User " + UserName + " successfully initialized new order " + order.GetOrderID() + ".");
            return order;
        }

        private Order InitOrder(OrderItem[] items)
        {
            GetUserDetailsFromBuyer();
            CheckAllItems(items);
            Order order = new Order(_orderDL.RandomOrderID(), UserName, UserAddress);
            foreach (OrderItem item in items)
            {
                order.AddOrderItem(item);
            }

            MarketLog.Log("OrderPool", "User " + UserName + " successfully initialized new order " + order.GetOrderID() + ".");
            return order;
        }

        private void GetUserDetailsFromBuyer()
        {
            if (_buyer.GetName() != null)
            {
                UserName = _buyer.GetName();
                UserAddress = _buyer.GetAddress();
                CreditCard = _buyer.GetCreditCard();
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

        public void UpdateUserDetails(string userName, string address, string creditCard)
        {
            UserName = userName;
            UserAddress = address;
            CreditCard = creditCard;
        }
    }
}
