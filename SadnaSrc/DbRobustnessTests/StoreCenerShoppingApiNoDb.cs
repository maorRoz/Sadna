using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;

namespace DbRobustnessTests
{
    [TestClass]
    public class StoreCenerShoppingApiNoDb
    {
        private IStoreShoppingService shoppingService;
        private MarketAnswer answer;
        [TestInitialize]
        public void BuildNoDataTest()
        {
            MarketDB.ToDisable = false;
            MarketDB.Instance.InsertByForce();
            var marketSession = MarketYard.Instance;
            var userService = marketSession.GetUserService();
            answer = userService.EnterSystem();
            Assert.AreEqual((int)EnterSystemStatus.Success, answer.Status);
            answer = userService.SignIn("Big Smoke", "123");
            Assert.AreEqual((int)SignInStatus.Success, answer.Status);
            shoppingService = marketSession.GetStoreShoppingService(ref userService);
        }
        [TestMethod]
        public void OpenStoreNoDBTest()
        {
            MarketDB.ToDisable = true;
            answer = shoppingService.OpenStore("We have no DB!", "who cares");
            Assert.AreEqual((int)OpenStoreStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void ViewStoreInfoNoDBTest()
        {
            MarketDB.ToDisable = true;
            answer = shoppingService.ViewStoreInfo("Cluckin Bell");
            Assert.AreEqual((int)ViewStoreStatus.NoDB, answer.Status);
        }


        [TestMethod]
        public void ViewStorStockNoDBTest()
        {
            MarketDB.ToDisable = true;
            answer = shoppingService.ViewStoreStock("Cluckin Bell");
            Assert.AreEqual((int)ViewStoreStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void ViewStorStockAllNoDBTest()
        {
            MarketDB.ToDisable = true;
            answer = shoppingService.ViewStoreStockAll("Cluckin Bell");
            Assert.AreEqual((int)ViewStoreStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void AddProductToCartNoDBTest()
        {
            MarketDB.ToDisable = true;
            answer = shoppingService.AddProductToCart("Cluckin Bell", "#9",1);
            Assert.AreEqual((int)AddProductStatus.NoDB, answer.Status);
        }

        [TestCleanup]
        public void CleanUpNoDataTest()
        {
            MarketDB.ToDisable = false;
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }



    }
}
