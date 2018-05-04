using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace IntegrationTests.StoreSyncher_Integration
{
    [TestClass]
    public class StoreCenter_OrderPool_IntegrationTest
    {
        private IUserService userServiceSession;
        private OrderService orderServiceSession;
        private StockSyncher storeServiceSession;
        private StoresSyncherHarmony storeSyncherHarmony;

        private MarketYard marketSession;
        private string store1 = "The Red Rock";
        private string store2 = "24";
        private string product1 = "Bamba";
        private string product2 = "Coated Peanuts";

        [TestInitialize]
        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService)marketSession.GetUserService();
            userServiceSession.EnterSystem();
            orderServiceSession = (OrderService)marketSession.GetOrderService(ref userServiceSession);
            storeServiceSession = StockSyncher.Instance;
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

        [TestMethod]
        public void GetPriceFromCouponTest()
        {
            try
            {
                Assert.AreEqual(60, storeSyncherHarmony.GetPriceFromCoupon("Pizza","The Red Rock", 2, "D1"));
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetPriceFromWrongCouponTest()
        {
            try
            {
                storeSyncherHarmony.GetPriceFromCoupon("Pizza", "The Red Rock", 2, "D3");
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void GetPriceFromNoCouponTest()
        {
            try
            {
                storeSyncherHarmony.GetPriceFromCoupon("Bamba", "The Red Rock", 2, "D3");
                Assert.Fail();
            }
            catch (MarketException)
            {
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
                orderServiceSession.LoginBuyer("Vadim Chernov", "123");
                orderServiceSession.GiveDetails("Vadim Chernov", "Mivtza Kilshon", "12345667");
                orderServiceSession.BuyItemFromImmediate(product1, store1, 3, 6, null);
                StockListItem itemToCheck = storeServiceSession.GetProductFromStore(store1, product1);
                Assert.AreEqual(17, itemToCheck.Quantity);
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
                orderServiceSession.LoginBuyer("Vova", "123");
                orderServiceSession.GiveDetails("Vova", "Donkelblum", "12345667");
                orderServiceSession.BuyItemFromImmediate(product2, store2, 8, 6, null);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }

            try
            {
                storeServiceSession.GetProductFromStore(store2, product1);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }

        }

        [TestMethod]
        public void EmptyCartStockUnchangedTest()
        {
            try
            {
                orderServiceSession.LoginBuyer("Arik1", "123");
                orderServiceSession.GiveDetails("Arik1", "AAA", "12345667");
                orderServiceSession.BuyItemFromImmediate(product1, store1, 3, 6, null);
                Assert.AreEqual(20, storeServiceSession.GetProductFromStore(store1, product1).Quantity);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetItemWithCouponTest()
        {
            try
            {
                orderServiceSession.LoginBuyer("CJ", "123");
                orderServiceSession.BuyItemFromImmediate("Pizza", "The Red Rock", 2, 60.00, "D1");
                Assert.AreEqual(8, storeServiceSession.GetProductFromStore("The Red Rock", "Pizza").Quantity);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetItemWrongCouponTest()
        {
            try
            {
                orderServiceSession.LoginBuyer("CJ", "123");
                orderServiceSession.BuyItemFromImmediate("Pizza", "The Red Rock", 2, 60.00, "D6");
                Assert.AreEqual(10, storeServiceSession.GetProductFromStore("The Red Rock", "Pizza").Quantity);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GetItemWithNoCouponTest()
        {
            try
            {
                orderServiceSession.LoginBuyer("Vadim Chernov", "123");
                orderServiceSession.BuyItemFromImmediate("Bamba", "The Red Rock", 2, 6.00, "D6");
                Assert.AreEqual(20, storeServiceSession.GetProductFromStore("The Red Rock", "Bamba").Quantity);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void BuyAllStockUpdateTest()
        {
            try
            {
                orderServiceSession.LoginBuyer("Vadim Chernov", "123");
                orderServiceSession.GiveDetails("Vadim Chernov", "Mivtza Kilshon", "12345667");
                orderServiceSession.BuyEverythingFromCart(new string[]{null, null, null});
                Assert.AreEqual(17, storeServiceSession.GetProductFromStore(store1, product1).Quantity);
                Assert.AreEqual(33, storeServiceSession.GetProductFromStore(store1, "Goldstar").Quantity);
                Assert.AreEqual(98, storeServiceSession.GetProductFromStore(store2, "OCB").Quantity);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestCleanup]
        public void StoreOrderTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
