using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using SadnaSrc.MarketData;

namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]

    public class ViewStoreHistoryTestsMock
    {
        private Mock<IStoreDL> handler;
        private Mock<IUserSeller> userService;
        private Mock<IMarketDB> marketDbMocker;
        private ViewStoreHistorySlave slave;




       [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserSeller>();
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);

            slave = new ViewStoreHistorySlave("X", userService.Object, handler.Object);

        }
        [TestMethod]
        public void NoStore()
        {
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
            slave.ViewStoreHistory();
            Assert.AreEqual((int)ManageStoreStatus.InvalidStore, slave.answer.Status);
        }
        [TestMethod]
        public void NoPermission()
        {
            userService.Setup(x => x.CanViewPurchaseHistory()).Throws(new MarketException(0, ""));
            slave.ViewStoreHistory();
            Assert.AreEqual((int)ManageStoreStatus.InvalidManager, slave.answer.Status);
        }

   
        [TestMethod]
        public void ViewStoreHistorySuccess()
        {

            ViewStoreHistorySlave slave = new ViewStoreHistorySlave("X", userService.Object, handler.Object);
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
            slave.ViewStoreHistory();
            MarketAnswer ans = slave.answer;
            Assert.AreEqual((int)ManageStoreStatus.Success, ans.Status);
        }
        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
