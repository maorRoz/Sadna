using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using System.Threading.Tasks;
using SadnaSrc.MarketData;

namespace StoreCenterTests.StoreCenterUnitTests
{
        [TestClass]
        public class ViewStoreInfoTestsMock
        {
            private Mock<IStoreDL> handler;
            private Mock<IUserShopper> userService;
            private Mock<IMarketDB> marketDbMocker;
            private ViewStoreInfoSlave slave;




        [TestInitialize]
            public void BuildStore()
            {
                marketDbMocker = new Mock<IMarketDB>();
                MarketException.SetDB(marketDbMocker.Object);
                MarketLog.SetDB(marketDbMocker.Object);
                handler = new Mock<IStoreDL>();
                userService = new Mock<IUserShopper>();
                handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);

            slave = new ViewStoreInfoSlave(userService.Object, handler.Object);
            }
            [TestMethod]
            public void NoStore()
            {
                handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
                slave.ViewStoreInfo("X");
            Assert.AreEqual((int)ViewStoreStatus.NoStore, slave.answer.Status);
            }
            [TestMethod]
            public void NoPermission()
            {
                userService.Setup(x => x.ValidateCanBrowseMarket()).Throws(new MarketException(0, ""));
                slave.ViewStoreInfo("X"); ;
                Assert.AreEqual((int)ManageStoreStatus.InvalidManager, slave.answer.Status);
            }


            [TestMethod]
            public void NullStore()
            {
                slave.ViewStoreInfo(null);
                Assert.AreEqual((int)ViewStoreStatus.NoStore, slave.answer.Status);
            }
            [TestMethod]
            public void ViewStorePass()
            {
            
                slave.ViewStoreInfo("X");
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
