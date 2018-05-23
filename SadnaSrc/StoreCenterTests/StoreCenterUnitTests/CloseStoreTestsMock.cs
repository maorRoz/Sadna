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
    // 
        [TestClass]
        public class CloseStoreTestsMock
        {
            private Mock<IStoreDL> handler;
            private Mock<IUserSeller> userService;
            private Mock<IMarketDB> marketDbMocker;
            private CloseStoreSlave slave;

            [TestInitialize]
            public void BuildStore()
            {
                marketDbMocker = new Mock<IMarketDB>();
                MarketException.SetDB(marketDbMocker.Object);
                MarketLog.SetDB(marketDbMocker.Object);
                handler = new Mock<IStoreDL>();
                userService = new Mock<IUserSeller>();
                handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
                handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
                slave = new CloseStoreSlave(userService.Object, "X", handler.Object);
            }
            [TestMethod]
            public void NoStore()
            {
                handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
                slave.CloseStore();
                Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);
            }

            [TestMethod]
            public void NoPermission()
            {
                userService.Setup(x => x.CanPromoteStoreOwner()).Throws(new MarketException(0, ""));
                slave.CloseStore();
                Assert.AreEqual((int)StoreEnum.CloseStoreFail, slave.answer.Status);
            }
        
            [TestMethod]
            public void CloseStorePass()
            {

                slave.CloseStore();
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