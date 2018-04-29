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
    public class PurchaseEverythingSlave : MakePurchaseSlave
    {
        public OrderAnswer Answer { get; private set; }

        public PurchaseEverythingSlave(IUserBuyer buyer, IStoresSyncher storesSync, IOrderDL orderDL) :
            base(buyer, storesSync, orderDL){}

        public Order BuyEverythingFromCart(string[] coupons, string UserName, string UserAddress, string CreditCard)
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
                Order order;
                try
                {
                    order = CreateOrderAllCart(UserName, UserAddress, coupons);
                    orderId = order.GetOrderID();
                }
                catch (MarketException e)
                {
                    MarketLog.Log("OrderPool", "Order " + orderId +
                                               " has failed to execute. Something is wrong with Store." +
                                               " Error message has been created!");
                    Answer = new OrderAnswer(OrderStatus.InvalidCoupon, e.GetErrorMessage());
                    return null;
                }
                ProcessOrder(order, CreditCard);
                _buyer.EmptyCart();
                MarketLog.Log("OrderPool", "User " + UserName + " successfully bought all the items in the cart.");
                Answer = new OrderAnswer(OrderStatus.Success, "Successfully bought all the items in the cart.");
                return order;
            }
            catch (OrderException e)
            {
                MarketLog.Log("OrderPool",
                    "Order " + orderId + " has failed to execute. Error message has been created!");
                Answer = new OrderAnswer((OrderStatus) e.Status, e.GetErrorMessage());
                return null;
            }
            catch (WalleterException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId +
                                           " has failed to execute while communicating with payment system." +
                                           " Error message has been created!");
                Answer = new OrderAnswer((WalleterStatus) e.Status, e.GetErrorMessage());
                return null;
            }
            catch (SupplyException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId +
                                           " has failed to execute while communicating with supply system." +
                                           " Error message has been created!");
                Answer = new OrderAnswer((SupplyStatus) e.Status, e.GetErrorMessage());
                return null;
            }
            catch (MarketException e)
            {
                MarketLog.Log("OrderPool", "Order " + orderId +
                                           " has failed to execute. Something is wrong with Store or User." +
                                           " Error message has been created!");
                Answer = new OrderAnswer(OrderStatus.InvalidUser, e.GetErrorMessage());
                return null;
            }
        }

        public Order InitOrder(OrderItem[] items, string UserName, string UserAddress)
        {
            CheckAllItems(items);
            Order order = new Order(_orderDL.RandomOrderID(), UserName, UserAddress);
            foreach (OrderItem item in items)
            {
                order.AddOrderItem(item);
            }

            MarketLog.Log("OrderPool",
                "User " + UserName + " successfully initialized new order " + order.GetOrderID() + ".");
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

        protected override void ProcessOrder(Order order, string CreditCard)
        {
            _supplyService.CreateDelivery(order);
            _paymentService.ProccesPayment(order, CreditCard);
            _storesSync.RemoveProducts(order.GetItems().ToArray());
            _orderDL.AddOrder(order);
        }

        private Order CreateOrderAllCart(string UserName, string UserAddress, string[] coupons)
        {
            OrderItem[] itemsToBuy = _buyer.CheckoutAll();
            if (coupons != null)
                for (int i = 0; i < itemsToBuy.Length; i++)
                {
                    if (coupons[i] != null)
                    {
                        itemsToBuy[i].Price = _storesSync.GetPriceFromCoupon(itemsToBuy[i].Name, itemsToBuy[i].Store, itemsToBuy[i].Quantity, coupons[i]);
                    }
                }
            Order order = InitOrder(itemsToBuy, UserName, UserAddress);
            return order;
        }
    }
}
