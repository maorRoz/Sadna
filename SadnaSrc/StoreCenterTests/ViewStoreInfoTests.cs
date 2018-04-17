using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCenterTests
{
    [TestClass]
    public class ViewStoreInfoTests
    {
        private MarketYard market;
        public StockListItem ProductToDelete;
        private ModuleGlobalHandler handler;
        IUserService userService;

        [TestInitialize]
        public void BuildStore()
        {

            market = MarketYard.Instance;
            handler = ModuleGlobalHandler.GetInstance();
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
            userService.CleanSession();
            MarketYard.CleanSession();
        }
    }
}
