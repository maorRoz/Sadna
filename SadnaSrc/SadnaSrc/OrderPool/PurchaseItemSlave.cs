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
    public class PurchaseItemSlave : MakePurchaseSlave
    {
        public OrderAnswer Answer { get; private set; }

        public PurchaseItemSlave(IUserBuyer buyer, IStoresSyncher storesSync, IOrderDL orderDL) : 
            base(buyer, storesSync, orderDL) {}

        public Order BuyItemFromImmediate(string itemName, string store, int quantity, double unitPrice, string coupon,
            string UserName, string UserAddress, string CreditCard)
        {
            if (UserName == null)
                UserName = _buyer.GetName();
            if (UserAddress == null)
                UserAddress = _buyer.GetAddress();
            if (CreditCard == null)
                CreditCard = _buyer.GetCreditCard();
            MarketLog.Log("OrderPool",
                "Attempting to buy " + quantity + " " + itemName + " from store " + store + " in immediate sale...");
            int orderId = 0;
            try
            {
                OrderItem toBuy = _buyer.CheckoutItem(itemName, store, quantity, unitPrice);
                if (coupon != null)
                {
                    try
                    {
                        toBuy.Price = _storesSync.GetPriceFromCoupon(itemName, store, quantity, coupon);
                    }
                    catch (MarketException e)
                    {
                        MarketLog.Log("OrderPool", "Order " + orderId +
                                                   " has failed to execute. Something is wrong with Store." +
                                                   " Error message has been created!");
                        Answer = new OrderAnswer(OrderStatus.InvalidCoupon, e.GetErrorMessage());
                        return null;
                    }
                }

                Order order = CreateOrderOneItem(toBuy, UserName, UserAddress);
                orderId = order.GetOrderID();
                ProcessOrder(order, CreditCard);
                _buyer.RemoveItemFromCart(itemName, store, quantity, unitPrice);
                MarketLog.Log("OrderPool",
                    "User " + UserName + " successfully bought item " + itemName + "in an immediate sale.");
                Answer = new OrderAnswer(OrderStatus.Success, "Successfully bought item " + itemName);
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
                                           " has failed to execute. Something is wrong with User." +
                                           " Error message has been created!");
                Answer = new OrderAnswer(OrderStatus.InvalidUser, e.GetErrorMessage());
                return null;
            }
        }

        public Order InitOrder(string UserName, string UserAddress)
        {
            Order order = new Order(_orderDL.RandomOrderID(), UserName, UserAddress);
            MarketLog.Log("OrderPool",
                "User " + UserName + " successfully initialized new order " + order.GetOrderID() + ".");
            return order;
        }

        private Order CreateOrderOneItem(OrderItem toBuy, string UserName,string UserAddress)
        {
            CheckOrderItem(toBuy);
            Order order = InitOrder(UserName, UserAddress);
            order.AddOrderItem(toBuy);
            return order;
        }

        protected override void ProcessOrder(Order order, string CreditCard)
        {
            _supplyService.CreateDelivery(order);
            _paymentService.ProccesPayment(order, CreditCard);
            _storesSync.RemoveProducts(order.GetItems().ToArray());
            _orderDL.AddOrder(order);
        }
    }
}
