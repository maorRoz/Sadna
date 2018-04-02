using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;
using SadnaSrc.SupplyPoint;
using SadnaSrc.UserSpot;
using SadnaSrc.Walleter;

namespace IntegrationTests
{
    [TestClass]
    public class UserOrderIntegration
    {
        private UserService userServiceSession;
        private OrderService orderServiceSession;
        private StoreService storeServiceSession;
        private MarketYard marketSession;
        private string userName = "Buyer1";
        private string userPass = "123";

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService)marketSession.GetUserService();
            storeServiceSession = (StoreService) marketSession.GetStoreService(userServiceSession);
            orderServiceSession =
                (OrderService) marketSession.GetOrderService(userServiceSession, storeServiceSession, new SupplyService(),
                    new PaymentService());
        }

        [TestMethod]
        public void TestConvertCartToOrder()
        {
            userServiceSession.EnterSystem();
            User currentUser = userServiceSession.GetUser();
            CartService cart = currentUser.Cart;
            cart.AddToCart("The Red Rock","Bamba", 6.90, "regular", 3);
            CartItem[] userCartItems = currentUser.GetCart();
            orderServiceSession.setUsername(""+ currentUser.SystemID);
            OrderItem[] userOrderItems = new OrderItem[userCartItems.Length];
            for (int i = 0; i < userCartItems.Length; i++)
            {
                userOrderItems[i] = new OrderItem(userCartItems[i]);
            }

            Order o = orderServiceSession.CreateOrder(userOrderItems);
            Assert.IsNotNull(orderServiceSession.FindOrderItemInOrder(o.GetOrderID(), "The Red Rock", 
                orderServiceSession.getUsername()));

        }
    }
}
