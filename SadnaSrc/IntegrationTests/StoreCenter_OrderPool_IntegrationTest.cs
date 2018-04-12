using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace IntegrationTests
{
    [TestClass]
    public class StoreCenter_OrderPool_IntegrationTest
    {
        private IUserService userServiceSession;
        private OrderService orderServiceSession;
        private ModuleGlobalHandler storeServiceSession;
        private StoresSyncherHarmony storeSyncherHarmony;

        private MarketYard marketSession;
        private string store = "The Red Rock";
        private string product1 = "Bamba";
        private string product2 = "OCB";
        private string pass = "123";

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService)marketSession.GetUserService();
            userServiceSession.EnterSystem();
            orderServiceSession = (OrderService)marketSession.GetOrderService(ref userServiceSession);
            storeServiceSession = ModuleGlobalHandler.GetInstance();
            storeSyncherHarmony = new StoresSyncherHarmony();
        }

        [TestMethod]
        public void UpdateStockAfterPurchaseTest()
        {
            try
            {
                orderServiceSession.BuyItemFromImmediate(product1, store, 10, 6);
                StockListItem itemToCheck = storeServiceSession.GetProductFromStore(store, product1);
                Assert.AreEqual(10, itemToCheck.Quantity);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
            
        }

        [TestMethod]
        public void ItemDeletedAfterPurchaseTest()
        {
            try
            {
                orderServiceSession.BuyItemFromImmediate(product2, store, 100, 6);
                StockListItem itemToCheck = storeServiceSession.GetProductFromStore(store, product1);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
            
        }

        [TestCleanup]
        public void StoreOrderTestCleanUp()
        {
            userServiceSession.CleanGuestSession();
            orderServiceSession.CleanSession();
            MarketYard.CleanSession();
        }
    }
}
