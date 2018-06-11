using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.MarketData;
using SadnaSrc.MarketRecovery;

namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]
    public class PromoteToManagerTestsMock
    {
        private Mock<IStoreDL> handler;
        private Mock<IUserSeller> userService;
        private Mock<IMarketBackUpDB> marketDbMocker;
        private PromoteToStoreManagerSlave slave;




        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketBackUpDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserSeller>();
            slave = new PromoteToStoreManagerSlave(userService.Object, "X", handler.Object);
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);

        }
        [TestMethod]
        public void NoStore()
        {
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
            slave.PromoteToStoreManager("X", "");
            Assert.AreEqual((int)PromoteStoreStatus.InvalidStore, slave.Answer.Status);
        }
        [TestMethod]
        public void NoPermission()
        {
            userService.Setup(x => x.CanPromoteStoreAdmin()).Throws(new MarketException((int)PromoteStoreStatus.InvalidStore, ""));

            slave.PromoteToStoreManager("X", "");
            Assert.AreEqual((int)PromoteStoreStatus.InvalidStore, slave.Answer.Status);
        }

        [TestMethod]
        public void PromoteMyself()
        {
            userService.Setup(x => x.ValidateNotPromotingHimself(It.IsAny<string>()))
                .Throws(new MarketException((int) PromoteStoreStatus.PromoteSelf, ""));
            slave.PromoteToStoreManager("bla", "");
            Assert.AreEqual((int)PromoteStoreStatus.PromoteSelf, slave.Answer.Status);
        }
        [TestMethod]
        public void PromoteToManagerSuccess()
        {
            slave.PromoteToStoreManager("bla", "");
            Assert.AreEqual((int)PromoteStoreStatus.Success, slave.Answer.Status);
        }

    }
}

/**
 * 
 * can't find a way to make thouse promotions fail...
[TestMethod]
public void PromoteToManagerNoUserFound()
{
    userService.EnterSystem();
    userService.SignIn("Arik3", "123");
    StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X", handler.Object);
    handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
    MarketAnswer ans = liorSession.PromoteToStoreManager("glimooiw", "StoreOwner");
    Assert.AreEqual((int)PromoteStoreStatus.NoUserFound, ans.Status);
}
[TestMethod]
public void PromoteToManagerNoAuthority()
{

    userService.EnterSystem();
    userService.SignIn("Vova", "123");
    StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X", handler.Object);
    handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
    MarketAnswer ans = liorSession.PromoteToStoreManager("Arik3", "StoreOwner");
    Assert.AreEqual((int)PromoteStoreStatus.NoAuthority, ans.Status);
}
[TestMethod]
public void PromoteToManagerSuccess()
{

    userService.EnterSystem();
    userService.SignIn("Arik2", "123");
    StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X", handler.Object);
    handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
    MarketAnswer ans = liorSession.PromoteToStoreManager("Arik3", "StoreOwner");
    Assert.AreEqual((int)PromoteStoreStatus.Success, ans.Status);
}
} */
