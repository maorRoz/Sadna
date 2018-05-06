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

namespace StoreCenterTests.StoreCenterUnitTests
{
        [TestClass]
        public class ViewStoreInfoTestsMock
        {
            private Mock<IStoreDL> handler;
            private Mock<IUserShopper> userService;
            private Mock<IMarketDB> marketDbMocker;



            //TODO: improve this

        [TestInitialize]
            public void BuildStore()
            {
                marketDbMocker = new Mock<IMarketDB>();
                MarketException.SetDB(marketDbMocker.Object);
                MarketLog.SetDB(marketDbMocker.Object);
                handler = new Mock<IStoreDL>();
                userService = new Mock<IUserShopper>();

            }
            [TestMethod]
            public void ViewStoreFail()
            {
                ViewStoreInfoSlave slave = new ViewStoreInfoSlave(userService.Object, handler.Object);
                slave.ViewStoreInfo("noStore");
                Assert.AreEqual((int)ViewStoreStatus.NoStore, slave.answer.Status);
            }
            [TestMethod]
            public void ViewStorePass()
            {
            
                handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", "bla"));
                handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
                ViewStoreInfoSlave slave = new ViewStoreInfoSlave(userService.Object, handler.Object);
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
