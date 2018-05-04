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
        Mock<IUserSeller> userService;
        [TestInitialize]
        public void BuildStore()
        {
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
    }
}
