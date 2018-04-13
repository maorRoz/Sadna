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

        /*
         * Standalone tests for StoreSyncher functions
         */

        [TestMethod]
        public void RemoveProductsTest()
        {
            try
            {
                OrderItem[] purchased = new OrderItem[] {new OrderItem(store, product1, 6, 10)};
                storeSyncherHarmony.RemoveProducts(purchased);
                Assert.AreEqual(10, storeServiceSession.GetProductFromStore(store, product1).Quantity);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void RemoveNonExistantProductsTest()
        {
            try
            {
                OrderItem[] purchased = new OrderItem[] { new OrderItem(store, "A" + product1, 6, 10) };
                storeSyncherHarmony.RemoveProducts(purchased);
                Assert.Fail();
            }
            catch (MarketException)
            {
                Assert.AreEqual(20, storeServiceSession.GetProductFromStore(store, product1).Quantity);
            }
        }

        [TestMethod]
        public void RemoveProductNonExistantStoreTest()
        {
            try
            {
                OrderItem[] purchased = new OrderItem[] { new OrderItem("A" + store, product1, 6, 10) };
                storeSyncherHarmony.RemoveProducts(purchased);
                Assert.Fail();
            }
            catch (MarketException)
            {
                Assert.AreEqual(20, storeServiceSession.GetProductFromStore(store, product1).Quantity);
            }
        }

        [TestMethod]
        public void RemoveProductLargeQuantityTest()
        {
            try
            {
                OrderItem[] purchased = new OrderItem[] { new OrderItem("A" + store, product1, 6, 100) };
                storeSyncherHarmony.RemoveProducts(purchased);
                Assert.Fail();
            }
            catch (MarketException)
            {
                Assert.AreEqual(20, storeServiceSession.GetProductFromStore(store, product1).Quantity);
            }
        }

        [TestMethod]
        public void IsValidItemTest()
        {
            try
            {
                Assert.IsTrue(storeSyncherHarmony.IsValid(new OrderItem(store, product1, 6, 10)));
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void IsValidNonExistantItemTest()
        {
            try
            {
                Assert.IsFalse(storeSyncherHarmony.IsValid(new OrderItem(store, "A" + product1, 6, 10)));
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void IsValidNonExistantStoreTest()
        {
            try
            {
                Assert.IsFalse(storeSyncherHarmony.IsValid(new OrderItem("A" + store, product1, 6, 10)));
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void IsValidlargeQuantityTest()
        {
            try
            {
                Assert.IsFalse(storeSyncherHarmony.IsValid(new OrderItem(store, product1, 6, 1000)));
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
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
