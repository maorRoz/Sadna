using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.MarketData;

namespace StoreCenterTests.StoreCenterDbIntegrationTests
{
    [TestClass]
    public class CalculateProductRealPriceTests
    {
        private MarketYard market;
        public Store toDeleteStore;
        private StockSyncher handler; //Need to be here
        IUserService userService;
        [TestInitialize]
        public void BuildStore()
        {
            MarketDB.Instance.InsertByForce();
            market = MarketYard.Instance;
            handler = StockSyncher.Instance;
            userService = market.GetUserService();
        }
        [TestMethod]
        public void CalculateWhenStoreNotExits()
        {
            try
            {
                handler.CalculateItemPriceWithDiscount("notAstore", "BOX", "D1", 1);
            }
            catch (StoreException exe)
            {
                Assert.AreEqual((int)CalculateEnum.StoreNotExists, exe.Status);
            }
        }
        [TestMethod]
        public void CalculateWhenProductNotExists()
        {
            try
            {
                handler.CalculateItemPriceWithDiscount("X", "notAProduct", "D1", 1);
            }
            catch (StoreException exe)
            {
                Assert.AreEqual((int)CalculateEnum.ProductNotFound, exe.Status);
            }
        }
        [TestMethod]
        public void CalculateWhenCodeIsIncorrect()
        {
            try
            {
                handler.CalculateItemPriceWithDiscount("X", "BOX", "D13", 1);
            }
            catch (StoreException exe)
            {
                Assert.AreEqual((int)CalculateEnum.DiscountCodeIsWrong, exe.Status);
            }
        }
        [TestMethod]
        public void CalculateWhenHasNoDiscount()
        {
            try
            {
                handler.CalculateItemPriceWithDiscount("T", "DeleteMy BOX", "D1", 1);
            }
            catch (StoreException exe)
            {
                Assert.AreEqual((int)CalculateEnum.ProductHasNoDiscount, exe.Status);
            }
        }
        [TestMethod]
        public void CalculateWhenQuantityIsTooBig()
        {
            try
            {
                handler.CalculateItemPriceWithDiscount("X", "BOX", "D1", 99999);
            }
            catch (StoreException exe)
            {
                Assert.AreEqual((int)CalculateEnum.QuantityIsGreaterThenStack, exe.Status);
            }
        }
        [TestMethod]
        public void CalculateWhenQuantityIsNegative()
        {
            try
            {
                handler.CalculateItemPriceWithDiscount("X", "BOX", "D1", -1);
            }
            catch (StoreException exe)
            {
                Assert.AreEqual((int)CalculateEnum.QuanitityIsNonPositive, exe.Status);
            }
        }
        [TestMethod]
        public void CalculateWhenQuantityIsZero()
        {
            try
            {
                handler.CalculateItemPriceWithDiscount("X", "BOX", "D1", 0);
            }
            catch (StoreException exe)
            {
                Assert.AreEqual((int)CalculateEnum.QuanitityIsNonPositive, exe.Status);
            }
        }
        [TestMethod]
        public void CalculateWhenDiscountExpiered()
        {//Alice
            try
            {
                handler.CalculateItemPriceWithDiscount("T", "TheHatter", "D3", 2);
            }
            catch (StoreException exe)
            {
                Assert.AreEqual((int)CalculateEnum.DiscountExpired, exe.Status);
            }
        }
        [TestMethod]
        public void CalculateWhenDiscountNotStarted()
        {
            try
            {
                handler.CalculateItemPriceWithDiscount("T", "Alice", "D2", 2);
            }
            catch (StoreException exe)
            {
                Assert.AreEqual((int)CalculateEnum.DiscountNotStarted, exe.Status);
            }
        }
        [TestMethod]
        public void CalculateWhenDiscountIsNotHidden()
        {
            try
            {
                handler.CalculateItemPriceWithDiscount("T", "WhiteRabbit", "D6", 2);
            }
            catch (StoreException exe)
            {
                Assert.AreEqual((int)CalculateEnum.DiscountIsNotHidden, exe.Status);
            }
        }
        [TestMethod]
        public void CalculateWhenSuccess()
        {
            try
            {
                double ans = handler.CalculateItemPriceWithDiscount("T", "LittleCake", "D4", 2);
                Assert.AreEqual(100, ans);
                ans = handler.CalculateItemPriceWithDiscount("T", "LittleCake", "D4", 1);
                Assert.AreEqual(50, ans);
                ans = handler.CalculateItemPriceWithDiscount("T", "LittleDrink", "D4", 1);
                Assert.AreEqual(100, ans);
                ans = handler.CalculateItemPriceWithDiscount("T", "CheshireCat", "D5", 1);
                Assert.AreEqual(150, ans);
            }
            catch (StoreException exe)
            {
                Assert.AreEqual((int)CalculateEnum.DiscountNotStarted, exe.Status);
            }

        }
        


        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
