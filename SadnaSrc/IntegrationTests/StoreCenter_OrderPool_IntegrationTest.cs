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
        private string store1 = "The Red Rock";
        private string store2 = "24";
        private string product1 = "Bamba";
        private string product2 = "Coated Peanuts";

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
                OrderItem[] purchased = new OrderItem[] {new OrderItem(store1, product1, 6, 10)};
                storeSyncherHarmony.RemoveProducts(purchased);
                Assert.AreEqual(10, storeServiceSession.GetProductFromStore(store1, product1).Quantity);
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
                OrderItem[] purchased = new OrderItem[] { new OrderItem(store1, "A" + product1, 6, 10) };
                storeSyncherHarmony.RemoveProducts(purchased);
                Assert.Fail();
            }
            catch (MarketException)
            {
                Assert.AreEqual(20, storeServiceSession.GetProductFromStore(store1, product1).Quantity);
            }
        }

        [TestMethod]
        public void RemoveProductNonExistantStoreTest()
        {
            try
            {
                OrderItem[] purchased = new OrderItem[] { new OrderItem("A" + store1, product1, 6, 10) };
                storeSyncherHarmony.RemoveProducts(purchased);
                Assert.Fail();
            }
            catch (MarketException)
            {
                Assert.AreEqual(20, storeServiceSession.GetProductFromStore(store1, product1).Quantity);
            }
        }

        [TestMethod]
        public void RemoveProductLargeQuantityTest()
        {
            try
            {
                OrderItem[] purchased = new OrderItem[] { new OrderItem("A" + store1, product1, 6, 100) };
                storeSyncherHarmony.RemoveProducts(purchased);
                Assert.Fail();
            }
            catch (MarketException)
            {
                Assert.AreEqual(20, storeServiceSession.GetProductFromStore(store1, product1).Quantity);
            }
        }

        [TestMethod]
        public void RemoveProductNegativeQuantityTest()
        {
            try
            {
                OrderItem[] purchased = new OrderItem[] { new OrderItem("A" + store1, product1, 6, -3) };
                storeSyncherHarmony.RemoveProducts(purchased);
                Assert.Fail();
            }
            catch (MarketException)
            {
                Assert.AreEqual(20, storeServiceSession.GetProductFromStore(store1, product1).Quantity);
            }
        }

        [TestMethod]
        public void IsValidItemTest()
        {
            try
            {
                Assert.IsTrue(storeSyncherHarmony.IsValid(new OrderItem(store1, product1, 6, 10)));
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
                storeSyncherHarmony.IsValid(new OrderItem(store1, "A" + product1, 6, 10));
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void IsValidNonExistantStoreTest()
        {
            try
            {
                Assert.IsFalse(storeSyncherHarmony.IsValid(new OrderItem("A" + store1, product1, 6, 10)));
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void IsValidLargeQuantityTest()
        {
            try
            {
                Assert.IsFalse(storeSyncherHarmony.IsValid(new OrderItem(store1, product1, 6, 1000)));
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void IsValidNegativeQuantityTest()
        {
            try
            {
                Assert.IsFalse(storeSyncherHarmony.IsValid(new OrderItem(store1, product1, 6, -30)));
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        /*
         * Actual integration tests
         */

        [TestMethod]
        public void UpdateStockAfterPurchaseTest()
        {
            try
            {
                userServiceSession.SignIn("Vadim Chernov", "123");
                orderServiceSession.GiveDetails("Vadim Chernov", "Mivtza Kilshon", "12345667");
                orderServiceSession.BuyItemFromImmediate(product1, store1, 10, 6);
                StockListItem itemToCheck = storeServiceSession.GetProductFromStore(store1, product1);
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
                userServiceSession.SignIn("Vova", "123");
                orderServiceSession.GiveDetails("Vova", "Donkelblum", "12345667");
                orderServiceSession.BuyItemFromImmediate(product2, store2, 100, 6);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }

            try
            {
                StockListItem itemToCheck = storeServiceSession.GetProductFromStore(store2, product1);
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
            storeServiceSession.DataLayer.RemoveStockListItem(storeServiceSession.GetProductFromStore(store1, product1));
            storeServiceSession.DataLayer.RemoveStockListItem(storeServiceSession.GetProductFromStore(store2, product2));
            MarketYard.CleanSession();
        }
    }
}
