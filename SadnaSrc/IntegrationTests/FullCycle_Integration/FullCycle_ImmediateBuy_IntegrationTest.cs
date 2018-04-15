using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace IntegrationTests.FullCycle_Integration
{

    [TestClass]
    public class FullCycle_ImmediateBuy_IntegrationTest
    {
        private IUserService userServiceSession;
        private IUserService userServiceSession2;
        private ISystemAdminService sysadminSession;
        private StoreShoppingService storeServiceSession;
        private StoreManagementService storeManagementSession;
        private OrderService orderServiceSession;
        private MarketYard marketSession;

        private string store = "The Red Rock";
        private string user = "Vova";
        private string sysadmin = "Arik1";
        private string pass = "123";
        private string product = "Goldstar";

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService)marketSession.GetUserService();
            userServiceSession2 = (UserService)marketSession.GetUserService();
            userServiceSession2.EnterSystem();
            userServiceSession2.SignIn(sysadmin, pass);
            sysadminSession = marketSession.GetSystemAdminService(userServiceSession2);
            userServiceSession.EnterSystem();
            orderServiceSession = (OrderService) marketSession.GetOrderService(ref userServiceSession);
            storeServiceSession = (StoreShoppingService) marketSession.GetStoreShoppingService(ref userServiceSession);

        }

        [TestMethod]
        public void GuestBuyItem()
        {
            try
            {
                storeServiceSession.MakeGuest();
                storeServiceSession.AddProductToCart(store, product, 3);
                orderServiceSession.GiveDetails("Someone", "Somewhere", "12345689");
                orderServiceSession.BuyItemFromImmediate(product, store, 3, 11);
                Assert.AreEqual(2, sysadminSession.ViewPurchaseHistoryByStore(store).ReportList.Length);
                string actual = sysadminSession.ViewPurchaseHistoryByStore(store).ReportList[1];
                Assert.AreEqual(PurchaseString("Someone"), actual);

            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void RegUserBuyItem()
        {
            try
            {
                storeServiceSession.LoginShoper(user, pass);
                storeServiceSession.AddProductToCart(store, product, 3);
                orderServiceSession.BuyItemFromImmediate(product, store, 3, 11);
                Assert.AreEqual(2, sysadminSession.ViewPurchaseHistoryByStore(store).ReportList.Length);
                string actual = sysadminSession.ViewPurchaseHistoryByStore(store).ReportList[1];
                Assert.AreEqual(PurchaseString(user), actual);

            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestCleanup]
        public void StoreOrderTestCleanUp()
        {
            userServiceSession.CleanSession();
            userServiceSession2.CleanSession();
            orderServiceSession.CleanSession();
            storeServiceSession.CleanSeesion();
            MarketYard.CleanSession();
        }

        private string PurchaseString(string buyer)
        {
            return "User: " + buyer + " Product: Goldstar Store: The Red Rock" + 
                   " Sale: Immediate Quantity: 3 Price: 33 Date: " + DateTime.Now.ToShortDateString();
        }
    }
}
