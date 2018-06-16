using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.StoreCenter;
using System;
using SadnaSrc.MarketData;


namespace StoreCenterTests.StoreCenterDbIntegrationTests
{
    [TestClass]
    public class ViewStoreStockTests
    {
        private MarketYard market;
        IUserService userService;
        IUserService userService2;
        [TestInitialize]
        public void BuildStore()
        {
            MarketDB.Instance.InsertByForce();
            market = MarketYard.Instance;
            userService = market.GetUserService();
            userService2 = market.GetUserService();
            MarketYard.SetDateTime(DateTime.Parse("14/04/2018"));
        }
        [TestMethod]
        public void ViewStoreStockWhenStoreNotExists()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.LoginShoper("Arik3", "123");
            MarketAnswer ans = liorSession.ViewStoreStock("STORE_NOT_EXISTS");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
        }
        [TestMethod]
        public void ViewStoreStockWhenHasNoPremmision()
        {

            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            MarketAnswer ans = liorSession.ViewStoreStock("X");
            Assert.AreEqual((int)StoreEnum.NoPermission, ans.Status);
        }
        [TestMethod]
        public void ViewStoreStockSuccess()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.LoginShoper("Arik3", "123");
            MarketAnswer ans = liorSession.ViewStoreStock("X");

            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
        }

        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
