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
        public class ViewStoreInfoTestsMock
        {
            private Mock<I_StoreDL> handler;
            Mock<IUserShopper> userService;

            [TestInitialize]
            public void BuildStore()
            {
                handler = new Mock<I_StoreDL>();
                userService = new Mock<IUserShopper>();

            }
            [TestMethod]
            public void AddToCartFail()
            {
                ViewStoreInfoSlave slave = new ViewStoreInfoSlave(userService.Object, handler.Object);
                slave.ViewStoreInfo("noStore");
                Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);
            }
            [TestMethod]
            public void AddToCartPass()
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
