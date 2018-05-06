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

namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]

    public class ViewStoreHistoryTestsMock
    {
        private Mock<IStoreDL> handler;
        private Mock<IUserSeller> userService;
        private Mock<IMarketDB> marketDbMocker;



        //TODO: improve this


        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserSeller>();
        }
        [TestMethod]
        public void ViewStoreHistoryStoreNotExists()
        {

            ViewStoreHistorySlave slave = new ViewStoreHistorySlave("X", userService.Object, handler.Object);
            slave.ViewStoreHistory();
            MarketAnswer ans = slave.answer;
            Assert.AreEqual((int)ManageStoreStatus.InvalidStore, ans.Status);
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
