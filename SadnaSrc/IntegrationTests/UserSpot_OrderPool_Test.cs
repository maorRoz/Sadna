using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.AdminView;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace IntegrationTests
{
    [TestClass]
    public class UserSpot_OrderPool_Test
    {
        private IUserService userServiceSession;
        private OrderService orderServiceSession;
        private StoreService storeServiceSession;
        private UserBuyerHarmony userBuyerHarmony;
        private UserShopperHarmony userShopperHarmony;
        private MarketYard marketSession;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService)marketSession.GetUserService();
            userServiceSession.EnterSystem();
            orderServiceSession = (OrderService)marketSession.GetOrderService(ref userServiceSession);
            storeServiceSession = (StoreService) marketSession.GetStoreShoppingService(ref userServiceSession);
            userBuyerHarmony = new UserBuyerHarmony(ref userServiceSession);
            userShopperHarmony = new UserShopperHarmony(ref userServiceSession);
        }

        [TestMethod]
        public void TestMakeOrderFromCart()
        {
            try
            {
                Product p = new Product("S1","Bamba",6,"munch");
                userShopperHarmony.AddToCart(p,"The Red Rock",3);
                OrderItem[] items = userBuyerHarmony.CheckoutAll();
                Order o = orderServiceSession.InitOrder(items);
                Assert.IsNotNull(orderServiceSession.FindOrderItemInOrder(o.GetOrderID(),"The Red Rock","Bamba"));
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void TestMakeOrderFromCartSeveralItems()
        {
            try
            {
                Product p1 = new Product("S1", "Bamba", 6, "munch");
                Product p2 = new Product("S2", "OCB", 10, "accessories");
                userShopperHarmony.AddToCart(p1, "The Red Rock", 3);
                userShopperHarmony.AddToCart(p2, "24", 2);
                OrderItem[] items = userBuyerHarmony.CheckoutAll();
                Order o = orderServiceSession.InitOrder(items);
                OrderItem[] orderItems = o.GetItems().ToArray();
                string result = "";
                for (int i=0;i<orderItems.Length;i++)
                {

                    result += "" + orderItems[i].Price + " " + orderItems[i].Name + ", " + orderItems[i].Store + ". ";
                }
                string expected = "20 OCB, 24. 18 Bamba, The Red Rock. ";

                Assert.AreEqual(result,expected);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestCleanup]
        public void UserOrderTestCleanUp()
        {
            userServiceSession.CleanGuestSession();
            orderServiceSession.CleanSession();
            userBuyerHarmony.CleanSession();
            MarketYard.CleanSession();
        }
    }
}
