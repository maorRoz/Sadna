using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCenterTests.StoreCenterDbIntegrationTests
{
    [TestClass]
    public class ViewStoreInfoTests
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
        public void ViewStoreStoreNotFound()
        {
            StoreShoppingService liorSession = (StoreShoppingService) market.GetStoreShoppingService(ref userService);
            liorSession.MakeGuest();
            MarketAnswer ans = liorSession.ViewStoreInfo("notStore");
            Assert.AreEqual((int) ViewStoreStatus.NoStore, ans.Status);
        }

        [TestMethod]
        public void ViewStoreNoPremission()
        {
            StoreShoppingService liorSession = (StoreShoppingService) market.GetStoreShoppingService(ref userService);
            MarketAnswer ans = liorSession.ViewStoreInfo("X");
            Assert.AreEqual((int) ViewStoreStatus.InvalidUser, ans.Status);
        }

        [TestMethod]
        public void ViewStoreSuccess()
        {
            StoreShoppingService liorSession = (StoreShoppingService) market.GetStoreShoppingService(ref userService);
            liorSession.MakeGuest();
            MarketAnswer ans = liorSession.ViewStoreInfo("X");
            Assert.AreEqual((int) StoreEnum.Success, ans.Status);
        }

        [TestCleanup]
        public void CleanUpTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
