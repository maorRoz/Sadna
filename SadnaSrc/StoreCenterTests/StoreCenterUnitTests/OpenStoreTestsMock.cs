﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]
    public class OpenStoreTestsMock
    {
        private Mock<IStoreDL> handler;
        private Mock<IUserShopper> userService;
        private Mock<IMarketDB> marketDbMocker;
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
        public void OpenStoreInterfaceLevelFail()
        {
            OpenStoreSlave slave = new OpenStoreSlave(userService.Object, handler.Object);
            handler.Setup(x => x.GetStorebyName("newStoreName")).Returns(new Store("newStoreName", ""));
            slave.OpenStore("newStoreName", "bla");
            Assert.AreEqual((int)OpenStoreStatus.AlreadyExist, slave.Answer.Status);
        }
        [TestMethod]
        public void OpenStoreInterfaceLevelPass()
        {
            OpenStoreSlave slave = new OpenStoreSlave(userService.Object, handler.Object);
            slave.OpenStore("newStoreName", "bla");
            Assert.AreEqual((int)OpenStoreStatus.Success, slave.Answer.Status);    
        }
        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }

    }
}
