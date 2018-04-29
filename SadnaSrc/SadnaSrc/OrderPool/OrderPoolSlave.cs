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
    public class OrderPoolSlave
    {
        private readonly IOrderDL _orderDL;

        private readonly IUserBuyer _buyer;
        private readonly IStoresSyncher _storesSync;

        private readonly SupplyService _supplyService;
        private readonly PaymentService _paymentService;

        private int cheatCode = -1;

        public OrderAnswer Answer { get; private set; }

        public OrderPoolSlave(ref IUserBuyer buyer, IStoresSyncher storesSync, IOrderDL orderDL)
        {
            _buyer = buyer;
            _storesSync = storesSync;
            _supplyService = SupplyService.Instance;
            _paymentService = PaymentService.Instance;
            _orderDL = orderDL;

            _supplyService.AttachExternalSystem();
            _paymentService.AttachExternalSystem();

        }

        public Order BuyItemFromImmediate(string itemName, string store, int quantity, double unitPrice, string UserName, string UserAddress, string CreditCard)
        {
            if (UserName == null)
                UserName = _buyer.GetName();
            if (UserAddress == null)
                UserAddress = _buyer.GetAddress();
            if (CreditCard == null)
                CreditCard = _buyer.GetCreditCard();
            MarketLog.Log("OrderPool", "Attempting to buy " + quantity + " " + itemName + " from store " + store + " in immediate sale...");
            int orderId = 0;
            try
            {
                OrderItem toBuy = _buyer.CheckoutItem(itemName, store, quantity, unitPrice);
                CheckOrderItem(toBuy);
                Order order = InitOrder(UserName, UserAddress);
                orderId = order.GetOrderID();
                order.AddOrderItem(toBuy);
                _supplyService.CreateDelivery(order);
                _paymentService.ProccesPayment(order, CreditCard);
                _orderDL.AddOrder(order);
                OrderItem[] wrap = { toBuy };
                _storesSync.RemoveProducts(wrap);
                _buyer.RemoveItemFromCart(itemName, store, quantity, unitPrice);
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
                MarketLog.Log("OrderPool", "Order " + orderId + " has failed to execute. Something is wrong with Store or User." +
                                           " Error message has been created!");
                Answer = new OrderAnswer(OrderStatus.InvalidUser, e.GetErrorMessage());
                return null;
            }
        }

        public Order BuyItemWithCoupon(string itemName, string store, int quantity, double unitPrice, string coupon, string UserName, string UserAddress, string CreditCard)
        {
            if (UserName == null)
                UserName = _buyer.GetName();
            if (UserAddress == null)
                UserAddress = _buyer.GetAddress();
            if (CreditCard == null)
                CreditCard = _buyer.GetCreditCard();
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
                Order order = InitOrder(UserName, UserAddress);
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

        public Order BuyAllItemsFromStore(string store, string UserName, string UserAddress, string CreditCard)
        {
            if (UserName == null)
                UserName = _buyer.GetName();
            if (UserAddress == null)
                UserAddress = _buyer.GetAddress();
            if (CreditCard == null)
                CreditCard = _buyer.GetCreditCard();
            MarketLog.Log("OrderPool", "Attempting to buy everything in cart from store " + store + "...");
            int orderId = 0;
            try
            {
                OrderItem[] itemsToBuy = _buyer.CheckoutFromStore(store);
                Order order = InitOrder(itemsToBuy, UserName, UserAddress);
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

        public Order BuyEverythingFromCart(string UserName, string UserAddress, string CreditCard)
        {
            if (UserName == null)
                UserName = _buyer.GetName();
            if (UserAddress == null)
                UserAddress = _buyer.GetAddress();
            if (CreditCard == null)
                CreditCard = _buyer.GetCreditCard();
            MarketLog.Log("OrderPool", "Attempting to buy everything in cart...");
            int orderId = 0;
            try
            {
                OrderItem[] itemsToBuy = _buyer.CheckoutAll();
                Order order = InitOrder(itemsToBuy, UserName, UserAddress);
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

        public Order BuyLotteryTicket(string itemName, string store, int quantity, double unitPrice, string UserName, string UserAddress, string CreditCard)
        {
            if (UserName == null)
                UserName = _buyer.GetName();
            if (UserAddress == null)
                UserAddress = _buyer.GetAddress();
            if (CreditCard == null)
                CreditCard = _buyer.GetCreditCard();
            MarketLog.Log("OrderPool", "Attempting to buy " + quantity + " tickets for lottery sale of " + itemName +
                                       " from store " + store + "...");
            int orderId = 0;
            try
            {
                _buyer.ValidateRegisteredUser();
                _storesSync.ValidateTicket(itemName, store, unitPrice);
                OrderItem ticketToBuy = new OrderItem(store, itemName, unitPrice, quantity);
                CheckOrderItem(ticketToBuy);
                Order order = InitOrder(UserName, UserAddress);
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

        public void GiveDetails(string userName, string address, string creditCard)
        {
            try
            {
                MarketLog.Log("OrderPool",
                    "User entering name and address for later usage in market order. validating data ...");
                IsValidUserDetails(userName, address, creditCard);
                MarketLog.Log("OrderPool", "Validation has been completed. User name and address are valid and been updated");
                Answer = new OrderAnswer(GiveDetailsStatus.Success, "User name and address has been updated successfully!");
            }
            catch (OrderException e)
            {
                Answer = new OrderAnswer(GiveDetailsStatus.InvalidNameOrAddress, e.GetErrorMessage());

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

        public Order InitOrder(string UserName, string UserAddress)
        {
            Order order = new Order(_orderDL.RandomOrderID(), UserName, UserAddress);
            MarketLog.Log("OrderPool", "User " + UserName + " successfully initialized new order " + order.GetOrderID() + ".");
            return order;
        }

        public Order InitOrder(OrderItem[] items, string UserName, string UserAddress)
        {
            CheckAllItems(items);
            Order order = new Order(_orderDL.RandomOrderID(), UserName, UserAddress);
            foreach (OrderItem item in items)
            {
                order.AddOrderItem(item);
            }

            MarketLog.Log("OrderPool", "User " + UserName + " successfully initialized new order " + order.GetOrderID() + ".");
            return order;
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

        private void IsValidUserDetails(string userName, string address, string creditCard)
        {
            int x;
            if (userName == null || address == null || creditCard == null || creditCard.Length != 8 || !Int32.TryParse(creditCard, out x))
            {
                MarketLog.Log("OrderPool", "User entered name or address which is invalid by the system standards!");
                throw new OrderException(GiveDetailsStatus.InvalidNameOrAddress, "User entered invalid name or address into the order");
            }
        }

        public void Cheat(int cheatResult)
        {
            cheatCode = cheatResult;
        }
    }
}
