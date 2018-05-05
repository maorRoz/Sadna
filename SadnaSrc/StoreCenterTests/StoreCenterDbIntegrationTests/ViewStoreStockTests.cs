using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCenterTests.StoreCenterDbIntegrationTestss
{
    [TestClass]
    public class ViewStoreStockTests
    {
        private MarketYard market;
        IUserService userService;
        [TestInitialize]
        public void BuildStore()
        {
            MarketDB.Instance.InsertByForce();
            market = MarketYard.Instance;
            userService = market.GetUserService();
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
            Assert.AreEqual((int)StoreEnum.NoPremmision, ans.Status);
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
