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

namespace StoreCenterTests
    {
        [TestClass]
        public class CloseStoreTestsMock
        {
            private Mock<I_StoreDL> handler;
            Mock<IUserSeller> userService;

            [TestInitialize]
            public void BuildStore()
            {
                handler = new Mock<I_StoreDL>();
                userService = new Mock<IUserSeller>();
            }
            [TestMethod]
            public void CloseStoreFail()
            {
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
            CloseStoreSlave slave = new CloseStoreSlave(userService.Object, "noStore", handler.Object);
            slave.CloseStore();
                Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);
            }
            [TestMethod]
            public void CloseStorePass()
            {
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
            handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", "bala"));
            CloseStoreSlave slave = new CloseStoreSlave(userService.Object, "X", handler.Object);
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