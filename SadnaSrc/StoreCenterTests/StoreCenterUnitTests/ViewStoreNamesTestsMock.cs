using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;

namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]
    public class ViewStoreNamesTestsMock
    {

        private Mock<IStoreDL> handler;
        private Mock<IUserShopper> userService;
        private Mock<IMarketDB> marketDbMocker;
        private ViewStoreNamesSlave slave;




        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserShopper>();
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);

            slave = new ViewStoreNamesSlave(handler.Object);

        }
       


        [TestMethod]
        public void ViewStoreHistorySuccess()
        {
            slave.ViewStores();
            Assert.AreEqual((int)StoreEnum.Success, slave.answer.Status);
        }
        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
